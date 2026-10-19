using Shouldly;

namespace External.Tests;

public class WhenARepoWasLastPushedBeforeBothWindows
{
    [Fact]
    public async Task Its_stats_are_never_fetched()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithARepoLastPushedBeforeBothWindows("abandoned").Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Requests.ShouldNotContain(r => r.RequestUri!.AbsolutePath.StartsWith("/repos/octo-dev/abandoned/"));
    }

    [Fact]
    public async Task It_is_left_out()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithARepoLastPushedBeforeBothWindows("abandoned").Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos.Count.ShouldBe(2);
    }
}
