using System.Diagnostics;
using Domain;
using External.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenTwoRepositoriesHaveColdStats
{
    [Fact]
    public async Task Their_stats_finish_within_one_retry_wait()
    {
        // Arrange
        var api = new GivenTheGitHubApi()
            .WithStatsStillComputing("ledger-sync", times: 1)
            .WithStatsStillComputing("side-project", times: 1)
            .Build();
        var metadata = new GitHubMetadata(
            new HttpClient(api) { BaseAddress = new Uri("https://api.github.com") },
            new GitHubOptions(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5)),
            TimeProvider.System);
        var elapsed = Stopwatch.StartNew();

        // Act
        await metadata.Fetch(GitHubClient.User, new PerEra<Window>(
            new(new DateOnly(2024, 12, 29), 26),
            new(new DateOnly(2026, 4, 19), 26)));

        // Assert
        elapsed.Elapsed.ShouldBeLessThan(TimeSpan.FromMilliseconds(1600));
    }
}
