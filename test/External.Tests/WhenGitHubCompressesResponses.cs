using Shouldly;

namespace External.Tests;

public class WhenGitHubCompressesResponses
{
    [Fact]
    public async Task The_fetch_still_reads_them()
    {
        // Arrange
        await using var server = new GivenTheGitHubApi().WithGzippedResponses().Listen();

        // Act
        var activity = await GitHubClient.FetchFrom(server);

        // Assert
        activity.Repos.Count.ShouldBe(2);
    }
}
