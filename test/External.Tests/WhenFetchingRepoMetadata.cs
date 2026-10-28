using Domain;
using Shouldly;

namespace External.Tests;

public class WhenFetchingRepoMetadata
{
    [Fact]
    public async Task Every_repo_the_user_can_see_is_listed()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos.Count.ShouldBe(2);
    }

    [Fact]
    public async Task A_private_repo_is_marked_private()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[1].IsPrivate.ShouldBeTrue();
    }

    [Fact]
    public async Task A_repo_s_creation_date_is_kept()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[1].CreatedOn.ShouldBe(new DateOnly(2026, 2, 1));
    }

    [Fact]
    public async Task Only_the_user_s_weeks_are_kept_whatever_the_login_s_case()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[0].AuthorWeeks
            .ShouldBe([new Week(new DateOnly(2025, 6, 1), 300, 30, 3), new Week(new DateOnly(2025, 6, 8), 120, 10, 4)]);
    }

    [Fact]
    public async Task Total_commits_count_every_author()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[0].TotalCommits.ShouldBe(9);
    }

    [Fact]
    public async Task The_punch_card_is_kept()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[1].PunchCard.ShouldBe([new PunchCell(6, 22, 5)]);
    }

    [Fact]
    public async Task Language_bytes_are_kept()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos[0].LanguageBytes["SQL"].ShouldBe(10_000);
    }

    [Fact]
    public async Task Requests_carry_the_user_s_token()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Requests.ShouldAllBe(r => r.Headers.Authorization!.ToString() == "Bearer user-token");
    }
}
