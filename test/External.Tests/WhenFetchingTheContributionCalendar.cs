using Shouldly;

namespace External.Tests;

public class WhenFetchingTheContributionCalendar
{
    [Fact]
    public async Task Only_days_with_a_contribution_are_kept()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.ContributionDays.ShouldBe([new DateOnly(2025, 6, 2), new DateOnly(2026, 6, 8)]);
    }

    [Fact]
    public async Task The_calendar_is_fetched_a_year_at_a_time()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Calls.Count(c => c.IsForCalendar).ShouldBe(2);
    }
}
