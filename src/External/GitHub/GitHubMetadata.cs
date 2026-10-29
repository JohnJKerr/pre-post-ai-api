using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Domain;
using Domain.GitHub;
using Microsoft.Extensions.Logging;

namespace External.GitHub;

public partial class GitHubMetadata(HttpClient http, GitHubOptions options, TimeProvider time, ILogger<GitHubMetadata> logger) : IGitHubMetadata
{
    // One contributionsCollection spans at most a year, and the two windows can be years
    // apart, so each window needs its own field. Aliases let both share a single round trip.
    private const string OneCalendarQuery = """
        query($from0: DateTime!, $to0: DateTime!) {
          viewer {
            w0: contributionsCollection(from: $from0, to: $to0) {
              contributionCalendar { weeks { contributionDays { date contributionCount } } }
            }
          }
        }
        """;

    private const string TwoCalendarQuery = """
        query($from0: DateTime!, $to0: DateTime!, $from1: DateTime!, $to1: DateTime!) {
          viewer {
            w0: contributionsCollection(from: $from0, to: $to0) {
              contributionCalendar { weeks { contributionDays { date contributionCount } } }
            }
            w1: contributionsCollection(from: $from1, to: $to1) {
              contributionCalendar { weeks { contributionDays { date contributionCount } } }
            }
          }
        }
        """;

    public async Task<GitHubActivity> Fetch(GitHubUser user, PerEra<Window> windows)
    {
        var metrics = new FetchMetrics();
        try
        {
            var started = Stopwatch.GetTimestamp();
            RepoResponse[] recent;
            try { recent = await RecentRepos(user, windows.Pre.Start); }
            finally { metrics.Listing = Stopwatch.GetElapsedTime(started); }
            metrics.RecentRepoCount = recent.Length;
            var statsDeadline = time.GetUtcNow() + options.StatsTimeout;
            RepoMetadata[] metadata;
            started = Stopwatch.GetTimestamp();
            try
            {
                var contributors = await GetStatsForRepos<ContributorResponse[]>(user, recent,
                    r => $"/repos/{r.FullName}/stats/contributors", statsDeadline, metrics);
                metadata = recent.Select((repo, index) => MetadataFor(user, repo, contributors[index] ?? [])).ToArray();
                var punchIndices = Enumerable.Range(0, recent.Length)
                    .Where(index => metadata[index].PunchCardEra(windows) is not null).ToArray();
                var punchRepos = punchIndices.Select(index => recent[index]).ToArray();
                var punchCards = await GetStatsForRepos<int[][]>(user, punchRepos,
                    r => $"/repos/{r.FullName}/stats/punch_card", statsDeadline, metrics);
                for (var i = 0; i < punchIndices.Length; i++)
                    metadata[punchIndices[i]] = metadata[punchIndices[i]] with
                    {
                        PunchCard = (punchCards[i] ?? []).Select(cell => new PunchCell(cell[0], cell[1], cell[2])).ToArray(),
                    };
            }
            finally { metrics.Stats = Stopwatch.GetElapsedTime(started); }

            started = Stopwatch.GetTimestamp();
            try
            {
                for (var i = 0; i < recent.Length; i++)
                    if (metadata[i].AuthorWeeks.Any(w => w.Added > 0 &&
                        (windows.Pre.Contains(w.Start) || windows.Post.Contains(w.Start))))
                        metadata[i] = metadata[i] with
                        {
                            LanguageBytes = await Get<Dictionary<string, long>>(user, $"/repos/{recent[i].FullName}/languages") ?? [],
                        };
            }
            finally { metrics.Languages = Stopwatch.GetElapsedTime(started); }

            started = Stopwatch.GetTimestamp();
            try { return new GitHubActivity(metadata, await ContributionDays(user, windows)); }
            finally { metrics.Calendars = Stopwatch.GetElapsedTime(started); }
        }
        finally
        {
            logger.LogInformation(
                "GitHub fetch: {RecentRepoCount} recent repos, {StatsPendingResponses} pending stats responses; listing {ListingMs:0} ms, stats {StatsMs:0} ms, languages {LanguagesMs:0} ms, calendars {CalendarsMs:0} ms.",
                metrics.RecentRepoCount, metrics.StatsPendingResponses,
                metrics.Listing.TotalMilliseconds, metrics.Stats.TotalMilliseconds,
                metrics.Languages.TotalMilliseconds, metrics.Calendars.TotalMilliseconds);
        }
    }

    private sealed class FetchMetrics
    {
        public int RecentRepoCount { get; set; }
        public int StatsPendingResponses { get; set; }
        public TimeSpan Listing { get; set; }
        public TimeSpan Stats { get; set; }
        public TimeSpan Languages { get; set; }
        public TimeSpan Calendars { get; set; }
    }

    // Listed most-recently-pushed first, so once the last repo on a page is older than
    // the pre window every later page is older too. A repo nobody has pushed to since
    // the pre window started can't have activity in either window.
    private const string RepoList = "/user/repos?per_page=100&sort=pushed&direction=desc";

    private async Task<RepoResponse[]> RecentRepos(GitHubUser user, DateOnly from)
    {
        var recent = new List<RepoResponse>();
        for (var page = RepoList; page is not null;)
        {
            using var response = await Send(user, page);
            var repos = await Read<RepoResponse[]>(response) ?? [];
            recent.AddRange(repos.Where(repo => PushedSince(repo, from)));
            var pageEndsBeforeWindow = repos.Length > 0 && !PushedSince(repos[^1], from);
            page = pageEndsBeforeWindow ? null : NextPage(response);
        }

        return recent.ToArray();
    }

    private static bool PushedSince(RepoResponse repo, DateOnly from) =>
        repo.PushedAt is { } pushedAt && DateOnly.FromDateTime(pushedAt.UtcDateTime) >= from;

    private async Task<IReadOnlyList<DateOnly>> ContributionDays(GitHubUser user, PerEra<Window> windows)
    {
        var requested = new[] { windows.Pre, windows.Post }.Where(window => window.Weeks > 0).ToArray();
        if (requested.Length == 0) return [];

        var calendars = await CalendarsFor(user, requested);
        return calendars
            .SelectMany(calendar => calendar.Weeks)
            .SelectMany(week => week.ContributionDays)
            .Where(day => day.ContributionCount > 0)
            .Select(day => day.Date)
            .ToArray();
    }

    private async Task<IReadOnlyList<ContributionCalendar>> CalendarsFor(GitHubUser user, IReadOnlyList<Window> windows)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/graphql");
        Prepare(request, user);
        request.Content = JsonContent.Create(new
        {
            query = windows.Count == 1 ? OneCalendarQuery : TwoCalendarQuery,
            variables = CalendarVariables(windows),
        });
        using var response = await http.SendAsync(request);
        var body = await Read<GraphQlResponse<CalendarsData>>(response);
        var viewer = body?.Data?.Viewer ?? throw new HttpRequestException("GitHub didn't return a contribution calendar.");

        return Enumerable.Range(0, windows.Count).Select(index =>
            viewer.GetValueOrDefault($"w{index}")?.ContributionCalendar
            ?? throw new HttpRequestException("GitHub didn't return a contribution calendar.")).ToArray();
    }

    private static Dictionary<string, string> CalendarVariables(IReadOnlyList<Window> windows)
    {
        var variables = new Dictionary<string, string>();
        for (var i = 0; i < windows.Count; i++)
        {
            variables[$"from{i}"] = string.Create(CultureInfo.InvariantCulture, $"{windows[i].Start:yyyy-MM-dd}T00:00:00Z");
            variables[$"to{i}"] = string.Create(CultureInfo.InvariantCulture, $"{windows[i].End:yyyy-MM-dd}T23:59:59Z");
        }

        return variables;
    }

    private static RepoMetadata MetadataFor(GitHubUser user, RepoResponse repo, ContributorResponse[] contributors)
    {
        var author = contributors.FirstOrDefault(c => string.Equals(c.Author?.Login, user.Handle, StringComparison.OrdinalIgnoreCase));

        return new RepoMetadata(
            repo.Private,
            DateOnly.FromDateTime(repo.CreatedAt.UtcDateTime),
            new Dictionary<string, long>(),
            author?.Weeks.Select(WeekFrom).ToArray() ?? [],
            contributors.Sum(c => c.Total),
            []);
    }

    private static Week WeekFrom(ContributorWeekResponse week) =>
        new(DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(week.W).UtcDateTime), week.A, week.D, week.C);

    // Ask for every repo's stats before waiting, while keeping HTTP calls serial.
    private async Task<T?[]> GetStatsForRepos<T>(GitHubUser user, IReadOnlyList<RepoResponse> repos,
        Func<RepoResponse, string> pathFor, DateTimeOffset deadline, FetchMetrics metrics) where T : class
    {
        var results = new T?[repos.Count];
        IReadOnlyList<int> pending = Enumerable.Range(0, repos.Count).ToArray();
        while (pending.Count > 0)
        {
            pending = await FetchStatsRound(user, repos, pending, results, pathFor, deadline, metrics);
            if (pending.Count > 0) await Task.Delay(options.StatsRetryDelay, time);
        }

        return results;
    }

    private async Task<IReadOnlyList<int>> FetchStatsRound<T>(GitHubUser user, IReadOnlyList<RepoResponse> repos,
        IReadOnlyList<int> pending, T?[] results, Func<RepoResponse, string> pathFor,
        DateTimeOffset deadline, FetchMetrics metrics) where T : class
    {
        var next = new List<int>();
        foreach (var index in pending)
        {
            var path = pathFor(repos[index]);
            using var response = await Send(user, path);
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                metrics.StatsPendingResponses++;
                if (time.GetUtcNow() >= deadline) throw new GitHubStatsPendingException(path);
                next.Add(index);
            }
            else results[index] = await Read<T>(response);
        }

        return next;
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
        Prepare(request, user);
        return await http.SendAsync(request);
    }

    private static void Prepare(HttpRequestMessage request, GitHubUser user) =>
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);

    private static async Task<T?> Read<T>(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized) throw new GitHubTokenRejectedException();
        response.EnsureSuccessStatusCode();
        if (response.StatusCode == HttpStatusCode.NoContent) return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    [GeneratedRegex("<(?<url>[^>]+)>;\\s*rel=\"next\"")]
    private static partial Regex NextLink();
}
