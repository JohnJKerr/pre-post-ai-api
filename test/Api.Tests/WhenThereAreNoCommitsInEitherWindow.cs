using System.Net;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenThereAreNoCommitsInEitherWindow(ApiFactory factory)
{
    [Fact]
    public async Task The_generation_is_empty()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(FakeGitHub.WithNoCommits(), gitHubOAuth: FakeGitHubOAuth.SigningInAs("quiet-dev", 10_201));
        var callback = await browser.SignInWithGitHub();

        // Act
        var generation = await browser.WaitForTheGeneration(callback);

        // Assert
        generation.Status.ShouldBe("empty");
    }

    [Fact]
    public async Task Nothing_is_saved()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(FakeGitHub.WithNoCommits(), gitHubOAuth: FakeGitHubOAuth.SigningInAs("silent-dev", 10_202));
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub());

        // Act
        var response = await factory.CreateClient().GetAsync("/api/reports/silent-dev");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
