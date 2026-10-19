using Shouldly;

namespace External.Tests;

public class WhenGitHubAnswersWithAnError
{
    [Fact]
    public async Task Fetching_stops()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithAnErrorFetching("ledger-sync").Build();

        // Act
        var fetching = () => GitHubClient.FetchFrom(api);

        // Assert
        await fetching.ShouldThrowAsync<HttpRequestException>();
    }
}
