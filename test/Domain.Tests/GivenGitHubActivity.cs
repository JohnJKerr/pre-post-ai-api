using Domain.GitHub;

namespace Domain.Tests;

internal class GivenGitHubActivity
{
    public const long GitHubId = 583_231;
    public static readonly DateTimeOffset Now = new(2026, 10, 18, 9, 0, 0, TimeSpan.Zero);
    public static readonly DateOnly PreWeek = new(2025, 6, 1);
    public static readonly DateOnly PostWeek = new(2026, 6, 7);
    public static readonly DateOnly BeforeBothWindows = new(2023, 1, 1);

    private readonly List<RepoMetadata> _repos = [];
    private readonly List<DateOnly> _contributionDays = [];

    public GivenGitHubActivity WithRepo(GivenAGitHubRepo repo)
    {
        _repos.Add(repo.Build());
        return this;
    }

    public GivenGitHubActivity WithContributionsOn(params DateOnly[] days)
    {
        _contributionDays.AddRange(days);
        return this;
    }

    public GitHubActivity Build() => new(_repos, _contributionDays);

    public Profile Assemble() => Profile.Assemble(GitHubId, "octo-dev", Cutoff.Default, Build(), Now);
}
