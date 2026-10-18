using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain;
using Domain.GitHub;

namespace External.GitHub;

public partial class GitHubMetadata(HttpClient http, GitHubOptions options) : IGitHubMetadata
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
        var metadata = new List<RepoMetadata>();
        foreach (var repo in repos) metadata.Add(await MetadataFor(user, repo));

        return new GitHubActivity(metadata, await ContributionDays(user, from, to));
    }

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

    private async Task<RepoMetadata> MetadataFor(GitHubUser user, RepoResponse repo)
    {
        var contributors = await GetStats<ContributorResponse[]>(user, $"/repos/{repo.FullName}/stats/contributors") ?? [];
        var punchCard = await GetStats<int[][]>(user, $"/repos/{repo.FullName}/stats/punch_card") ?? [];
        var languages = await Get<Dictionary<string, long>>(user, $"/repos/{repo.FullName}/languages") ?? [];
        var author = contributors.FirstOrDefault(c => string.Equals(c.Author?.Login, user.Handle, StringComparison.OrdinalIgnoreCase));

        return new RepoMetadata(
            repo.Name,
            repo.Private,
            DateOnly.FromDateTime(repo.CreatedAt.UtcDateTime),
            languages,
            author?.Weeks.Select(WeekFrom).ToArray() ?? [],
            contributors.Sum(c => c.Total),
            punchCard.Select(cell => new PunchCell(cell[0], cell[1], cell[2])).ToArray());
    }

    private static Week WeekFrom(ContributorWeekResponse week) =>
        new(DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(week.W).UtcDateTime), week.A, week.D, week.C);

    private async Task<T?> GetStats<T>(GitHubUser user, string path)
    {
        for (var attempt = 1; attempt <= options.StatsAttempts; attempt++)
        {
            using var response = await Send(user, path);
            if (response.StatusCode != HttpStatusCode.Accepted) return await Read<T>(response);

            await Task.Delay(options.StatsRetryDelay);
        }

        throw new GitHubStatsPendingException(path);
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
        response.EnsureSuccessStatusCode();
        return response.StatusCode == HttpStatusCode.NoContent ? default : await response.Content.ReadFromJsonAsync<T>();
    }

    [GeneratedRegex("<(?<url>[^>]+)>;\\s*rel=\"next\"")]
    private static partial Regex NextLink();
}
