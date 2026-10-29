using Domain;
using Shouldly;

namespace External.Tests;

public class WhenTheScoringWindowsAreFarApart
{
    [Fact]
    public async Task Only_the_two_scoring_windows_need_calendars()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();
        var windows = new PerEra<Window>(
            new(new DateOnly(2022, 5, 29), 26),
            new(new DateOnly(2026, 4, 19), 26));

        // Act
        await GitHubClient.Over(api).Fetch(GitHubClient.User, windows);

        // Assert
        api.Requests.Count(r => r.RequestUri!.AbsolutePath == "/graphql").ShouldBe(1);
    }
}
