using Shouldly;

namespace External.Tests;

public class WhenARepoWasLastPushedBeforeBothWindows
{
    [Fact]
    public async Task Its_commits_are_never_fetched()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithARepoLastPushedBeforeBothWindows("abandoned").Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Calls.ShouldNotContain(c => c.IsForHistory && c.Variable("name") == "abandoned");
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

    [Fact]
    public async Task No_more_pages_of_repos_are_fetched()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithARepoLastPushedBeforeBothWindows("abandoned").Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Calls.Count(c => c.IsForRepos).ShouldBe(1);
    }
}
