using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubRejectsTheTokenDuringGeneration(ApiFactory factory)
{
    [Fact]
    public async Task The_generation_fails()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(FakeGitHub.RejectingTheToken(), gitHubOAuth: FakeGitHubOAuth.SigningInAs("revoked-dev", 10_401));
        var callback = await browser.SignInWithGitHub();

        // Act
        var generation = await browser.WaitForTheGeneration(callback);

        // Assert
        generation.Status.ShouldBe("failed");
    }

    [Fact]
    public async Task The_message_asks_them_to_sign_in_again()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(FakeGitHub.RejectingTheToken(), gitHubOAuth: FakeGitHubOAuth.SigningInAs("revoked-dev", 10_401));
        var callback = await browser.SignInWithGitHub();

        // Act
        var generation = await browser.WaitForTheGeneration(callback);

        // Assert
        generation.Message.ShouldBe("GitHub didn't accept your sign-in. Sign in again.");
    }
}
