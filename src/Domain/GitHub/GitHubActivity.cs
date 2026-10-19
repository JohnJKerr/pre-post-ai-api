namespace Domain.GitHub;

// CommitTimes are the user's own commits at their local time, across every repo.
public record GitHubActivity(
    IReadOnlyList<RepoMetadata> Repos,
    IReadOnlyList<DateOnly> ContributionDays,
    IReadOnlyList<DateTime> CommitTimes,
    IReadOnlyList<DateOnly> PullRequestsMergedOn);
