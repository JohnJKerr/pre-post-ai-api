using External.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenWaitingForGitHubToComputeStats
{
    [Fact]
    public void A_repo_gets_five_minutes_to_be_ready()
    {
        // Arrange
        var options = GitHubOptions.Default;

        // Act
        var wait = options.StatsRetryDelay * options.StatsAttempts;

        // Assert
        wait.ShouldBe(TimeSpan.FromMinutes(5));
    }
}
