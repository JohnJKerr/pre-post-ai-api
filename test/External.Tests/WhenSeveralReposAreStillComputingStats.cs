using Domain.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenSeveralReposAreStillComputingStats
{
    [Fact]
    public async Task Fetching_gives_up_once_five_minutes_have_passed_in_all()
    {
        // Arrange
        var api = new GivenTheGitHubApi()
            .WithStatsStillComputing("ledger-sync", times: 40)
            .WithStatsStillComputing("side-project", times: 40)
            .Build();

        // Act
        var fetching = () => GitHubClient.FetchFrom(api);

        // Assert
        await fetching.ShouldThrowAsync<GitHubStatsPendingException>();
    }
}
