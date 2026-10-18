using Shouldly;

namespace External.Tests;

public class WhenTheUserHasMoreReposThanFitOnAPage
{
    [Fact]
    public async Task Repos_on_every_page_are_listed()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithASecondPageOfRepos().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos.Count.ShouldBe(3);
    }
}
