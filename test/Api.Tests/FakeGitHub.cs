using Domain;
using Domain.GitHub;

namespace Api.Tests;

internal class FakeGitHub(Exception? failure = null, bool hasCommits = true) : IGitHubMetadata
{
    public static FakeGitHub WithNoCommits() => new(hasCommits: false);

    public static FakeGitHub StillComputingStats() => new(new GitHubStatsPendingException("octo-dev/agent-runner"));

    public static FakeGitHub RejectingTheToken() => new(new GitHubTokenRejectedException());

    private static readonly RepoMetadata Repo = new(
        IsPrivate: false,
        new DateOnly(2026, 1, 10),
        new Dictionary<string, long> { ["TypeScript"] = 9_000 },
        [new Week(new DateOnly(2025, 6, 1), 500, 100, 5), new Week(new DateOnly(2026, 6, 7), 2_000, 300, 20)],
        TotalCommits: 25,
        [new PunchCell(2, 21, 25)]);

    public Task<GitHubActivity> Fetch(GitHubUser user, PerEra<Window> windows) => failure is null
        ? Task.FromResult(hasCommits ? new GitHubActivity([Repo], [new DateOnly(2026, 6, 8)]) : new GitHubActivity([], []))
        : throw failure;
}
