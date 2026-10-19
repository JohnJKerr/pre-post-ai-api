using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSigningInWithACutoff(ApiFactory factory)
{
    [Fact]
    public async Task The_report_compares_either_side_of_it()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("early-adopter", 10_101));
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub(cutoff: "2022-11-30"));

        // Act
        var report = await factory.CreateClient().GetFromJsonAsync<ReportResponse>("/api/reports/early-adopter");

        // Assert
        report!.Cutoff.ShouldBe(new CutoffResponse("2022-11-30"));
    }
}
