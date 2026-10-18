using System.Net;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenAUserRenamesTheirGitHubAccount(ApiFactory factory)
{
    [Fact]
    public async Task Their_new_handle_shows_their_report()
    {
        // Arrange
        await GenerateAReportAs("before-rename", 7_001);

        // Act
        await GenerateAReportAs("after-rename", 7_001);

        // Assert
        (await factory.CreateClient().GetAsync("/api/reports/after-rename")).StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Their_old_handle_no_longer_has_a_report()
    {
        // Arrange
        await GenerateAReportAs("was-renamed", 7_002);

        // Act
        await GenerateAReportAs("is-renamed", 7_002);

        // Assert
        (await factory.CreateClient().GetAsync("/api/reports/was-renamed")).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private async Task GenerateAReportAs(string login, long gitHubId)
    {
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs(login, gitHubId));
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub());
    }
}
