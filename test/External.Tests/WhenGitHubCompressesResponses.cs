using Shouldly;

namespace External.Tests;

public class WhenGitHubCompressesResponses
{
    [Fact]
    public async Task The_fetch_still_reads_them()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithGzippedResponses().Build();

        // Act
        var activity = await GitHubClient.FetchFrom(api);

        // Assert
        activity.Repos.Count.ShouldBe(2);
    }
}
