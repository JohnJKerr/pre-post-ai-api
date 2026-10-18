using Domain.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenGitHubRejectsTheUsersToken
{
    [Fact]
    public async Task Fetching_stops_saying_the_user_must_sign_in_again()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithTheTokenExpiredBeforeFetching("ledger-sync").Build();

        // Act
        var fetching = () => GitHubClient.FetchFrom(api);

        // Assert
        await fetching.ShouldThrowAsync<GitHubTokenRejectedException>();
    }
}
