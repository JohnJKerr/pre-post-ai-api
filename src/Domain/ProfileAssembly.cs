using Domain.GitHub;

namespace Domain;

internal class ProfileAssembly(GitHubActivity activity, PerEra<Window> windows)
{
    private const double PunchCardAuthorship = 0.9;

    private readonly PerEra<int> _addedInWindow = new(
        AddedIn(activity.Repos, windows.Pre),
        AddedIn(activity.Repos, windows.Post));

    public IReadOnlyList<Repo> Repos() => activity.Repos.Select(RepoFrom).ToArray();

    public WindowActivity ActivityIn(Era era)
    {
        var window = windows[era];
        var weeks = window.WeekStarts.Select(start => WeekAcrossRepos(start)).ToArray();
        var activeDays = activity.ContributionDays.Distinct().Count(window.Contains);
        var eligible = activity.Repos.Where(r => PunchCardEraOf(r) == era).ToArray();
        var commits = CommitsIn(activity.Repos, window);
        var punchShare = commits > 0 ? (double)CommitsIn(eligible, window) / commits : 0;

        return new WindowActivity(weeks, PunchCardAcross(eligible), activeDays, punchShare);
    }

    private Repo RepoFrom(RepoMetadata repo) => new(
        Name: null,
        repo.IsPrivate,
        ByteShares(repo.LanguageBytes),
        new PerEra<double>(ShareOfLines(repo, Era.Pre), ShareOfLines(repo, Era.Post)),
        PunchCardEraOf(repo),
        StartedIn(repo.CreatedOn));

    private Era? StartedIn(DateOnly createdOn) =>
        windows.Pre.Contains(createdOn) ? Era.Pre
        : windows.Post.Contains(createdOn) ? Era.Post
        : null;

    private Era? PunchCardEraOf(RepoMetadata repo)
    {
        var authored = repo.AuthorWeeks.Sum(w => w.Commits);
        if (repo.TotalCommits == 0 || (double)authored / repo.TotalCommits < PunchCardAuthorship) return null;

        var activeWeeks = repo.AuthorWeeks.Where(w => w.Commits > 0).ToArray();
        var cutoffWeekStart = windows.Post.Start;
        Era? side = activeWeeks.All(w => w.Start < cutoffWeekStart) ? Era.Pre
            : activeWeeks.All(w => w.Start >= cutoffWeekStart) ? Era.Post
            : null;

        return side is { } era && CommitsIn([repo], windows[era]) > 0 ? era : null;
    }

    private static IReadOnlyList<PunchCell> PunchCardAcross(IReadOnlyList<RepoMetadata> repos)
    {
        var commitsByHour = repos
            .SelectMany(r => r.PunchCard)
            .GroupBy(c => (c.Day, c.Hour))
            .ToDictionary(g => g.Key, g => g.Sum(c => c.Commits));

        return Enumerable.Range(0, 7 * 24)
            .Select(i => (Day: i / 24, Hour: i % 24))
            .Select(slot => new PunchCell(slot.Day, slot.Hour, commitsByHour.GetValueOrDefault(slot)))
            .ToArray();
    }

    private static int CommitsIn(IEnumerable<RepoMetadata> repos, Window window) =>
        repos.SelectMany(r => r.AuthorWeeks).Where(w => window.Contains(w.Start)).Sum(w => w.Commits);

    private Week WeekAcrossRepos(DateOnly start)
    {
        var weeks = activity.Repos.SelectMany(r => r.AuthorWeeks).Where(w => w.Start == start).ToArray();
        return new Week(start, weeks.Sum(w => w.Added), weeks.Sum(w => w.Deleted), weeks.Sum(w => w.Commits));
    }

    private double ShareOfLines(RepoMetadata repo, Era era) =>
        _addedInWindow[era] > 0 ? (double)AddedIn([repo], windows[era]) / _addedInWindow[era] : 0;

    private static int AddedIn(IEnumerable<RepoMetadata> repos, Window window) =>
        repos.SelectMany(r => r.AuthorWeeks).Where(w => window.Contains(w.Start)).Sum(w => w.Added);

    private static IReadOnlyDictionary<string, double> ByteShares(IReadOnlyDictionary<string, long> bytes)
    {
        var total = (double)bytes.Values.Sum();
        return bytes.ToDictionary(b => b.Key, b => total > 0 ? b.Value / total : 0);
    }
}
