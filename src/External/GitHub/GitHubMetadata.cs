using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain;
using Domain.GitHub;

namespace External.GitHub;

// Everything comes from GraphQL: unlike the REST stats endpoints, it never answers 202 while GitHub
// computes, and it carries each commit's own UTC offset, so the punch card is in the user's local time.
// Listing commits is the slow part, about a second per page, so a few repos are fetched at once: far
// below GraphQL's secondary limits, and a call GitHub does slow down is retried after the wait it asks for.
public class GitHubMetadata(HttpClient http) : IGitHubMetadata
{
    private const int ReposAtOnce = 4;
    private const int RateLimitedAttempts = 3;
    private static readonly TimeSpan DefaultRateLimitWait = TimeSpan.FromMinutes(1);

    private const string ReposQuery = """
        query($cursor: String) {
          viewer {
            id
            repositories(first: 100, after: $cursor, ownerAffiliations: [OWNER, COLLABORATOR, ORGANIZATION_MEMBER], orderBy: { field: PUSHED_AT, direction: DESC }) {
              pageInfo { hasNextPage endCursor }
              nodes {
                owner { login }
                name
                isPrivate
                createdAt
                pushedAt
                languages(first: 20, orderBy: { field: SIZE, direction: DESC }) { edges { size node { name } } }
              }
            }
          }
        }
        """;

    private const string HistoryQuery = """
        query($owner: String!, $name: String!, $author: ID!, $since: GitTimestamp!, $until: GitTimestamp!, $cursor: String) {
          repository(owner: $owner, name: $name) {
            defaultBranchRef {
              target {
                ... on Commit {
                  all: history(since: $since, until: $until) { totalCount }
                  mine: history(first: 100, after: $cursor, since: $since, until: $until, author: { id: $author }) {
                    pageInfo { hasNextPage endCursor }
                    nodes { author { date } additions deletions }
                  }
                }
              }
            }
          }
        }
        """;

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
        var (viewerId, repos) = await ReposPushedSince(user, from);
        var metadata = new RepoMetadata[repos.Count];
        await Parallel.ForEachAsync(
            Enumerable.Range(0, repos.Count),
            new ParallelOptions { MaxDegreeOfParallelism = ReposAtOnce },
            async (index, _) => metadata[index] = await MetadataFor(user, viewerId, repos[index], from, to));

        return new GitHubActivity(metadata, await ContributionDays(user, from, to));
    }

    // Repos come most recently pushed first, so the first one nobody has pushed to since the pre
    // window started ends the list: it and every repo after it can't have activity in either window.
    private async Task<(string ViewerId, IReadOnlyList<RepoNode> Repos)> ReposPushedSince(GitHubUser user, DateOnly from)
    {
        var repos = new List<RepoNode>();
        string? viewerId = null;
        for (string? cursor = null; ;)
        {
            var viewer = (await Query<ReposData>(user, ReposQuery, new { cursor })).Viewer;
            viewerId = viewer.Id;
            var recent = viewer.Repositories.Nodes.TakeWhile(r => PushedSince(r, from)).ToArray();
            repos.AddRange(recent);
            if (recent.Length < viewer.Repositories.Nodes.Count || !viewer.Repositories.PageInfo.HasNextPage) return (viewerId, repos);
            cursor = viewer.Repositories.PageInfo.EndCursor;
        }
    }

    private static bool PushedSince(RepoNode repo, DateOnly from) =>
        repo.PushedAt is { } pushedAt && DateOnly.FromDateTime(pushedAt.UtcDateTime) >= from;

    private async Task<RepoMetadata> MetadataFor(GitHubUser user, string viewerId, RepoNode repo, DateOnly from, DateOnly to)
    {
        var commits = new List<CommitNode>();
        var totalCommits = 0;
        for (string? cursor = null; ;)
        {
            var history = (await Query<HistoryData>(user, HistoryQuery, new
            {
                owner = repo.Owner.Login,
                name = repo.Name,
                author = viewerId,
                since = StartOf(from),
                until = EndOf(to),
                cursor,
            })).Repository?.DefaultBranchRef?.Target;
            if (history is null) break;

            totalCommits = history.All.TotalCount;
            commits.AddRange(history.Mine.Nodes);
            if (!history.Mine.PageInfo.HasNextPage) break;
            cursor = history.Mine.PageInfo.EndCursor;
        }

        return new RepoMetadata(
            repo.IsPrivate,
            DateOnly.FromDateTime(repo.CreatedAt.UtcDateTime),
            repo.Languages.Edges.ToDictionary(e => e.Node.Name, e => e.Size),
            WeeksOf(commits),
            totalCommits,
            PunchCardOf(commits));
    }

    // Weeks start on Sunday in UTC, as GitHub's own weekly stats do.
    private static IReadOnlyList<Week> WeeksOf(IEnumerable<CommitNode> commits) => commits
        .GroupBy(c => WeekStart(DateOnly.FromDateTime(c.Author.Date.UtcDateTime)))
        .OrderBy(week => week.Key)
        .Select(week => new Week(week.Key, week.Sum(c => c.Additions), week.Sum(c => c.Deletions), week.Count()))
        .ToArray();

    private static DateOnly WeekStart(DateOnly date) => date.AddDays(-(int)date.DayOfWeek);

    private static IReadOnlyList<PunchCell> PunchCardOf(IEnumerable<CommitNode> commits) => commits
        .GroupBy(c => (Day: (int)c.Author.Date.DayOfWeek, c.Author.Date.Hour))
        .Select(cell => new PunchCell(cell.Key.Day, cell.Key.Hour, cell.Count()))
        .ToArray();

    private async Task<IReadOnlyList<DateOnly>> ContributionDays(GitHubUser user, DateOnly from, DateOnly to)
    {
        var days = new List<DateOnly>();
        for (var start = from; start <= to; start = start.AddYears(1))
        {
            var end = Min(start.AddYears(1).AddDays(-1), to);
            var calendar = (await Query<CalendarData>(user, CalendarQuery, new { from = StartOf(start), to = EndOf(end) }))
                .Viewer.ContributionsCollection.ContributionCalendar;
            days.AddRange(calendar.Weeks.SelectMany(w => w.ContributionDays).Where(d => d.ContributionCount > 0).Select(d => d.Date));
        }

        return days;
    }

    private static DateOnly Min(DateOnly a, DateOnly b) => a < b ? a : b;

    private static string StartOf(DateOnly day) => string.Create(CultureInfo.InvariantCulture, $"{day:yyyy-MM-dd}T00:00:00Z");

    private static string EndOf(DateOnly day) => string.Create(CultureInfo.InvariantCulture, $"{day:yyyy-MM-dd}T23:59:59Z");

    private async Task<T> Query<T>(GitHubUser user, string query, object variables) where T : class
    {
        using var response = await Send(user, query, variables);
        if (response.StatusCode == HttpStatusCode.Unauthorized) throw new GitHubTokenRejectedException();
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<GraphQlResponse<T>>();
        return body?.Data ?? throw new HttpRequestException($"GitHub answered with an error: {body?.Errors?.FirstOrDefault()?.Message}");
    }

    private async Task<HttpResponseMessage> Send(GitHubUser user, string query, object variables)
    {
        for (var attempt = 1; ; attempt++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/graphql");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
            request.Content = JsonContent.Create(new { query, variables });
            var response = await http.SendAsync(request);
            if (attempt == RateLimitedAttempts || RateLimitWait(response) is not { } wait) return response;

            response.Dispose();
            await Task.Delay(wait);
        }
    }

    // GitHub signals a rate limit with 429, or with 403 plus Retry-After or no requests remaining.
    // Any other 403 is a real refusal and isn't retried.
    private static TimeSpan? RateLimitWait(HttpResponseMessage response)
    {
        var limited = response.StatusCode == HttpStatusCode.TooManyRequests
            || (response.StatusCode == HttpStatusCode.Forbidden
                && (response.Headers.RetryAfter is not null || Header(response, "x-ratelimit-remaining") == "0"));
        if (!limited) return null;

        if (response.Headers.RetryAfter?.Delta is { } delta) return delta;
        if (long.TryParse(Header(response, "x-ratelimit-reset"), out var reset))
            return TimeSpan.FromSeconds(Math.Max(0, reset - DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
        return DefaultRateLimitWait;
    }

    private static string? Header(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out var values) ? values.FirstOrDefault() : null;
}
