using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubSendsTheUserBackAfterInstallingTheApp(ApiFactory factory)
{
    private const string InstallCallback = "/api/auth/callback?code=github-code&installation_id=123&setup_action=install";

    [Fact]
    public async Task Sign_in_restarts()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();

        // Act
        var callback = await browser.GetAsync(InstallCallback);

        // Assert
        callback.Headers.Location!.ToString().ShouldBe("/api/auth/sign-in");
    }

    [Fact]
    public async Task Their_report_starts_generating()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("new-installer", 10_601));
        var callback = await browser.GetAsync(InstallCallback);

        // Act
        var signedIn = await browser.AuthoriseOnGitHubFrom(callback.Headers.Location!.ToString());

        // Assert
        signedIn.Headers.Location!.ToString().ShouldStartWith($"{ApiFactory.FrontEndOrigin}/generating?id=");
    }
}
