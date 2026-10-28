using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain;
using Domain.GitHub;

namespace External.GitHub;

public partial class GitHubMetadata(HttpClient http, GitHubOptions options, TimeProvider time) : IGitHubMetadata
{
    private const string CalendarQuery = """
        query($from: DateTime!, $to: DateTime!) {
          viewer {
            contributionsCollection(from: $from, to: $to) {
              contributionCalendar { weeks { contributionDays { date contributionCount } } }
            }
          }
        }
        """;

    public async Task<GitHubActivity> Fetch(GitHubUser user, DateOnly from, DateOnly to)
    {
        var repos = await GetEveryPage<RepoResponse>(user, "/user/repos?per_page=100");
        var recent = repos.Where(r => PushedSince(r, from)).ToArray();
        var metadata = new List<RepoMetadata>();
        var statsDeadline = time.GetUtcNow() + options.StatsTimeout;
        var contributors = await GetStatsForRepos<ContributorResponse[]>(user, recent,
            r => $"/repos/{r.FullName}/stats/contributors", statsDeadline);
        var punchCards = await GetStatsForRepos<int[][]>(user, recent,
            r => $"/repos/{r.FullName}/stats/punch_card", statsDeadline);
        for (var i = 0; i < recent.Length; i++)
            metadata.Add(await MetadataFor(user, recent[i], contributors[i] ?? [], punchCards[i] ?? []));

        return new GitHubActivity(metadata, await ContributionDays(user, from, to));
    }

    // Each repo costs three calls. A repo nobody has pushed
    // to since the pre window started can't have activity in either window.
    private static bool PushedSince(RepoResponse repo, DateOnly from) =>
        repo.PushedAt is { } pushedAt && DateOnly.FromDateTime(pushedAt.UtcDateTime) >= from;

    private async Task<IReadOnlyList<DateOnly>> ContributionDays(GitHubUser user, DateOnly from, DateOnly to)
    {
        var days = new List<DateOnly>();
        for (var start = from; start <= to; start = start.AddYears(1))
        {
            var end = Min(start.AddYears(1).AddDays(-1), to);
            var calendar = await CalendarBetween(user, start, end);
            days.AddRange(calendar.Weeks.SelectMany(w => w.ContributionDays).Where(d => d.ContributionCount > 0).Select(d => d.Date));
        }

        return days;
    }

    private async Task<ContributionCalendar> CalendarBetween(GitHubUser user, DateOnly start, DateOnly end)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/graphql");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
        request.Content = JsonContent.Create(new
        {
            query = CalendarQuery,
            variables = new
            {
                from = string.Create(CultureInfo.InvariantCulture, $"{start:yyyy-MM-dd}T00:00:00Z"),
                to = string.Create(CultureInfo.InvariantCulture, $"{end:yyyy-MM-dd}T23:59:59Z"),
            },
        });
        using var response = await http.SendAsync(request);
        var body = await Read<GraphQlResponse<CalendarData>>(response);

        return body?.Data?.Viewer.ContributionsCollection.ContributionCalendar
            ?? throw new HttpRequestException("GitHub didn't return a contribution calendar.");
    }

    private static DateOnly Min(DateOnly a, DateOnly b) => a < b ? a : b;

    private async Task<RepoMetadata> MetadataFor(GitHubUser user, RepoResponse repo, ContributorResponse[] contributors, int[][] punchCard)
    {
        var languages = await Get<Dictionary<string, long>>(user, $"/repos/{repo.FullName}/languages") ?? [];
        var author = contributors.FirstOrDefault(c => string.Equals(c.Author?.Login, user.Handle, StringComparison.OrdinalIgnoreCase));

        return new RepoMetadata(
            repo.Private,
            DateOnly.FromDateTime(repo.CreatedAt.UtcDateTime),
            languages,
            author?.Weeks.Select(WeekFrom).ToArray() ?? [],
            contributors.Sum(c => c.Total),
            punchCard.Select(cell => new PunchCell(cell[0], cell[1], cell[2])).ToArray());
    }

    private static Week WeekFrom(ContributorWeekResponse week) =>
        new(DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(week.W).UtcDateTime), week.A, week.D, week.C);

    // Ask for every repo's stats before waiting, while keeping HTTP calls serial.
    private async Task<T?[]> GetStatsForRepos<T>(GitHubUser user, IReadOnlyList<RepoResponse> repos,
        Func<RepoResponse, string> pathFor, DateTimeOffset deadline) where T : class
    {
        var results = new T?[repos.Count];
        var pending = Enumerable.Range(0, repos.Count).ToArray();
        while (pending.Length > 0)
        {
            var next = new List<int>();
            foreach (var index in pending)
            {
                var path = pathFor(repos[index]);
                using var response = await Send(user, path);
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    if (time.GetUtcNow() >= deadline) throw new GitHubStatsPendingException(path);
                    next.Add(index);
                }
                else results[index] = await Read<T>(response);
            }

            if (next.Count == 0) break;
            await Task.Delay(options.StatsRetryDelay, time);
            pending = next.ToArray();
        }

        return results;
    }

    private async Task<IReadOnlyList<T>> GetEveryPage<T>(GitHubUser user, string firstPage)
    {
        var items = new List<T>();
        for (var page = firstPage; page is not null;)
        {
            using var response = await Send(user, page);
            items.AddRange(await Read<T[]>(response) ?? []);
            page = NextPage(response);
        }

        return items;
    }

    private static string? NextPage(HttpResponseMessage response)
    {
        var links = response.Headers.TryGetValues("Link", out var values) ? string.Join(",", values) : "";
        var next = NextLink().Match(links);
        return next.Success ? next.Groups["url"].Value : null;
    }

    private async Task<T?> Get<T>(GitHubUser user, string path)
    {
        using var response = await Send(user, path);
        return await Read<T>(response);
    }

    private async Task<HttpResponseMessage> Send(GitHubUser user, string path)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
        return await http.SendAsync(request);
    }

    private static async Task<T?> Read<T>(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized) throw new GitHubTokenRejectedException();
        response.EnsureSuccessStatusCode();
        return response.StatusCode == HttpStatusCode.NoContent ? default : await response.Content.ReadFromJsonAsync<T>();
    }

    [GeneratedRegex("<(?<url>[^>]+)>;\\s*rel=\"next\"")]
    private static partial Regex NextLink();
}
