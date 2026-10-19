using Shouldly;

namespace External.Tests;

public class WhenARepoIsEmpty
{
    [Fact]
    public async Task It_has_no_weeks()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithEmptyRepo("side-project").Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[0].AuthorWeeks.ShouldBeEmpty();
    }

    [Fact]
    public async Task It_has_no_punch_card()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithEmptyRepo("side-project").Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[0].PunchCard.ShouldBeEmpty();
    }
}
