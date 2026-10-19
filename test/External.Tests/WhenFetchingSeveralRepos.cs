using Shouldly;

namespace External.Tests;

public class WhenFetchingSeveralRepos
{
    [Fact]
    public async Task Their_commits_are_fetched_at_the_same_time()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.MostCallsAtOnce.ShouldBe(2);
    }

    [Fact]
    public async Task They_keep_the_order_GitHub_listed_them_in()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos.Select(r => r.CreatedOn).ShouldBe([new DateOnly(2026, 2, 1), new DateOnly(2025, 3, 14)]);
    }
}
