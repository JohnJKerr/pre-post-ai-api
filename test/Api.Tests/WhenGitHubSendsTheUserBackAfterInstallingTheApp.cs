using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubSendsTheUserBackAfterInstallingTheApp(ApiFactory factory)
{
    private const string InstallCallback = "/api/auth/callback?code=github-code&installation_id=123&setup_action=install";

    [Fact]
    public async Task Sign_in_restarts_for_the_generate_page()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();

        // Act
        var callback = await browser.GetAsync(InstallCallback);

        // Assert
        callback.Headers.Location!.ToString().ShouldBe("/api/auth/sign-in?returnUrl=%2Fgenerate");
    }

    [Fact]
    public async Task They_end_up_signed_in_on_the_generate_page()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();
        var callback = await browser.GetAsync(InstallCallback);

        // Act
        var signedIn = await browser.AuthoriseOnGitHubFrom(callback.Headers.Location!.ToString());

        // Assert
        signedIn.Headers.Location!.ToString().ShouldBe($"{ApiFactory.FrontEndOrigin}/generate");
    }
}
