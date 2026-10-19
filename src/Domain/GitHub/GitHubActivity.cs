namespace Domain.GitHub;

public record GitHubActivity(IReadOnlyList<RepoMetadata> Repos, IReadOnlyList<DateOnly> ContributionDays);
