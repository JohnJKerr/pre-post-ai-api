using Domain.GitHub;
using External.GitHub;

namespace External.Tests;

internal static class GitHubClient
{
    public static readonly GitHubUser User = new("octo-dev", "user-token");

    public static GitHubMetadata Over(FakeGitHubApi api) =>
        new(new HttpClient(api) { BaseAddress = new Uri("https://api.github.com") }, GitHubOptions.NoDelay);

    public static Task<GitHubActivity> FetchFrom(FakeGitHubApi api) =>
        Over(api).Fetch(User, new DateOnly(2024, 12, 29), new DateOnly(2026, 10, 17));
}
