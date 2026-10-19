using Shouldly;

namespace External.Tests;

public class WhenFetchingMergedPullRequests
{
    [Fact]
    public async Task The_day_each_was_merged_is_kept()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.PullRequestsMergedOn.ShouldBe([new DateOnly(2026, 6, 13), new DateOnly(2025, 6, 11)]);
    }

    [Fact]
    public async Task Only_the_user_s_pull_requests_merged_within_the_windows_are_searched_for()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Calls.Single(c => c.IsForPullRequests).Variable("search").ShouldBe("is:pr is:merged author:octo-dev merged:2024-12-29..2026-10-17");
    }

    [Fact]
    public async Task Pull_requests_on_every_page_are_kept()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithASecondPageOfMergedPullRequests().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.PullRequestsMergedOn.Count.ShouldBe(3);
    }
}
