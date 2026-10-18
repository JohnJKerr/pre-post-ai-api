using Shouldly;

namespace External.Tests;

public class WhenGitHubIsStillComputingStats
{
    [Fact]
    public async Task The_stats_are_fetched_once_ready()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithStatsStillComputing("ledger-sync", times: 2).Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos.Single(r => r.Name == "ledger-sync").AuthorWeeks.Count.ShouldBe(2);
    }
}
