using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubIsStillComputingStats(ApiFactory factory)
{
    [Fact]
    public async Task The_generation_fails()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(FakeGitHub.StillComputingStats(), gitHubOAuth: FakeGitHubOAuth.SigningInAs("busy-dev", 10_301));
        var callback = await browser.SignInWithGitHub();

        // Act
        var generation = await browser.WaitForTheGeneration(callback);

        // Assert
        generation.Status.ShouldBe("failed");
    }

    [Fact]
    public async Task The_message_asks_them_to_try_again_shortly()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(FakeGitHub.StillComputingStats(), gitHubOAuth: FakeGitHubOAuth.SigningInAs("busy-dev", 10_301));
        var callback = await browser.SignInWithGitHub();

        // Act
        var generation = await browser.WaitForTheGeneration(callback);

        // Assert
        generation.Message.ShouldBe("GitHub is still crunching your repository stats. Sign in again in a minute to retry.");
    }
}
