using Shouldly;

namespace External.Tests;

public class WhenCallingGitHub
{
    [Fact]
    public async Task The_request_asks_for_gzip()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Requests.ShouldAllBe(request => request.Headers.AcceptEncoding.Any(encoding => encoding.Value == "gzip"));
    }
}
