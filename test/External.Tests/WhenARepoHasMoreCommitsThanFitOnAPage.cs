using Shouldly;

namespace External.Tests;

public class WhenARepoHasMoreCommitsThanFitOnAPage
{
    [Fact]
    public async Task Commits_on_every_page_are_counted()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithASecondPageOfCommits("ledger-sync").Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[1].AuthorWeeks.Sum(w => w.Commits).ShouldBe(8);
    }
}
