using Domain;
using Domain.GitHub;
using External.GitHub;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace External.Tests;

internal static class GitHubClient
{
    public static readonly GitHubUser User = new(583_231, "octo-dev", "user-token");
    public static readonly PerEra<Window> Windows = new(
        new(new DateOnly(2024, 12, 29), 26),
        new(new DateOnly(2026, 4, 19), 26));

    // Retries don't wait, but each look at the clock moves it on by one retry delay.
    public static GitHubMetadata Over(FakeGitHubApi api) =>
        new(new HttpClient(api) { BaseAddress = new Uri("https://api.github.com") },
            GitHubOptions.NoDelay,
            new FakeTimeProvider { AutoAdvanceAmount = GitHubOptions.Default.StatsRetryDelay },
            NullLogger<GitHubMetadata>.Instance);

    public static Task<GitHubActivity> FetchFrom(FakeGitHubApi api) =>
        Over(api).Fetch(User, Windows);

    public static async Task<GitHubActivity> FetchFrom(ListeningGitHub server)
    {
        using var client = new HttpClient(GitHubHttp.PrimaryHandler()) { BaseAddress = server.BaseAddress };
        return await new GitHubMetadata(client, GitHubOptions.NoDelay,
            new FakeTimeProvider { AutoAdvanceAmount = GitHubOptions.Default.StatsRetryDelay },
            NullLogger<GitHubMetadata>.Instance).Fetch(User, Windows);
    }
}
