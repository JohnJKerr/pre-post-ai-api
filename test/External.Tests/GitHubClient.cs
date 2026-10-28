using Domain;
using Domain.GitHub;
using External.GitHub;
using Microsoft.Extensions.Time.Testing;

namespace External.Tests;

internal static class GitHubClient
{
    public static readonly GitHubUser User = new(583_231, "octo-dev", "user-token");

    // Retries don't wait, but each look at the clock moves it on by one retry delay.
    public static GitHubMetadata Over(FakeGitHubApi api) =>
        new(new HttpClient(api) { BaseAddress = new Uri("https://api.github.com") },
            GitHubOptions.NoDelay,
            new FakeTimeProvider { AutoAdvanceAmount = GitHubOptions.Default.StatsRetryDelay });

    public static Task<GitHubActivity> FetchFrom(FakeGitHubApi api) =>
        Over(api).Fetch(User, new PerEra<Window>(
            new(new DateOnly(2024, 12, 29), 26),
            new(new DateOnly(2026, 4, 19), 26)));
}
