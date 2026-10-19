using Shouldly;

namespace External.Tests;

public class WhenFetchingCommitTimes
{
    [Fact]
    public async Task Every_commit_of_the_user_s_is_kept_at_their_local_time()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.CommitTimes.Count(t => t == new DateTime(2025, 6, 3, 10, 15, 0)).ShouldBe(1);
    }

    [Fact]
    public async Task Commits_from_every_repo_are_kept()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.CommitTimes.Count.ShouldBe(12);
    }
}
