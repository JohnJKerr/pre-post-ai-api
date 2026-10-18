using Domain.GitHub;

namespace Domain;

internal class ProfileAssembly(GitHubActivity activity, PerEra<Window> windows)
{
    private readonly PerEra<int> _addedInWindow = new(
        AddedIn(activity.Repos, windows.Pre),
        AddedIn(activity.Repos, windows.Post));

    public IReadOnlyList<Repo> Repos() => activity.Repos.Select(RepoFrom).ToArray();

    public WindowActivity ActivityIn(Era era)
    {
        var window = windows[era];
        var weeks = window.WeekStarts.Select(start => WeekAcrossRepos(start)).ToArray();
        var activeDays = activity.ContributionDays.Distinct().Count(window.Contains);
        return new WindowActivity(weeks, [], activeDays, PunchShare: 0);
    }

    private Repo RepoFrom(RepoMetadata repo) => new(
        repo.Name,
        repo.IsPrivate,
        ByteShares(repo.LanguageBytes),
        new PerEra<double>(ShareOfLines(repo, Era.Pre), ShareOfLines(repo, Era.Post)),
        PunchCardEra: null,
        StartedIn(repo.CreatedOn));

    private Era? StartedIn(DateOnly createdOn) =>
        windows.Pre.Contains(createdOn) ? Era.Pre
        : windows.Post.Contains(createdOn) ? Era.Post
        : null;

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
