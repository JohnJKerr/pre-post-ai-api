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
        activity.Repos[1].AuthorWeeks.ShouldBeEmpty();
    }

    [Fact]
    public async Task It_has_no_punch_card()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithEmptyRepo("side-project").Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[1].PunchCard.ShouldBeEmpty();
    }

    [Fact]
    public async Task Its_languages_are_not_requested()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithEmptyRepo("side-project").Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Requests.ShouldNotContain(r => r.RequestUri!.AbsolutePath == "/repos/octo-dev/side-project/languages");
    }
}
