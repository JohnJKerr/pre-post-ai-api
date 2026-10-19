using Shouldly;

namespace External.Tests;

public class WhenGitHubSaysToSlowDown
{
    [Fact]
    public async Task The_call_is_retried_after_the_wait_GitHub_asks_for()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithASecondaryRateLimitFetching("ledger-sync").Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[1].AuthorWeeks.Count.ShouldBe(2);
    }
}
