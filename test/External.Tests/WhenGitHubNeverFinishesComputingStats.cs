using Domain.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenGitHubNeverFinishesComputingStats
{
    [Fact]
    public async Task Fetching_gives_up_saying_the_stats_are_pending()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithStatsNeverReady("ledger-sync").Build();

        // Act
        var fetching = () => GitHubClient.FetchFrom(api);

        // Assert
        await fetching.ShouldThrowAsync<GitHubStatsPendingException>();
    }
}
