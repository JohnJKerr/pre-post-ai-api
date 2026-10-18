using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSomeoneTakesARenamedUsersOldHandle(ApiFactory factory)
{
    [Fact]
    public async Task The_handle_shows_the_new_owner_s_report()
    {
        // Arrange
        var formerOwner = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("passed-on", 8_001));
        await formerOwner.WaitForTheGeneration(await formerOwner.SignInWithGitHub());
        var newOwner = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("passed-on", 8_002));

        // Act
        await newOwner.WaitForTheGeneration(await newOwner.SignInWithGitHub(cutoff: "2022-11-30"));

        // Assert
        (await factory.CreateClient().GetFromJsonAsync<ReportResponse>("/api/reports/passed-on"))!.Cutoff.Label.ShouldBe("ChatGPT launch");
    }
}
