using Shouldly;

namespace External.Tests;

public class WhenCallingGitHub
{
    [Fact]
    public async Task The_request_asks_for_gzip()
    {
        // Arrange
        await using var server = new GivenTheGitHubApi().Listen();

        // Act
        await GitHubClient.FetchFrom(server);

        // Assert
        server.Requests.ShouldAllBe(request => request.Headers.AcceptEncoding.Any(encoding => encoding.Value == "gzip"));
    }
}
