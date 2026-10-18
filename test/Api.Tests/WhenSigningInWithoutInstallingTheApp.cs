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
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.WithoutTheAppInstalled("uninstalled-dev"));

        // Act
        var callback = await browser.SignInWithGitHub();

        // Assert
        callback.Headers.Location!.ToString().ShouldBe("https://github.com/apps/pre-post-test/installations/new");
    }

    [Fact]
    public async Task No_report_is_generated()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.WithoutTheAppInstalled("uninstalled-dev"));

        // Act
        await browser.SignInWithGitHub();

        // Assert
        (await factory.CreateClient().GetAsync("/api/reports/uninstalled-dev")).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
