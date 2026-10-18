using System.Net;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSigningInWithoutInstallingTheApp(ApiFactory factory)
{
    [Fact]
    public async Task They_are_sent_to_install_it()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.WithoutTheAppInstalled());

        // Act
        var callback = await browser.SignInWithGitHub(returnUrl: "/generate");

        // Assert
        callback.Headers.Location!.ToString().ShouldBe("https://github.com/apps/pre-post-test/installations/new");
    }

    [Fact]
    public async Task They_are_not_signed_in()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.WithoutTheAppInstalled());

        // Act
        await browser.SignInWithGitHub(returnUrl: "/generate");

        // Assert
        (await browser.GetAsync("/api/session")).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
