using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenRegeneratingAReport(ApiFactory factory)
{
    [Fact]
    public async Task The_earlier_report_is_replaced()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("Second-Go", 10_501));
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub());

        // Act
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub(cutoff: "2022-11-30"));

        // Assert
        (await factory.CreateClient().GetFromJsonAsync<ReportResponse>("/api/reports/second-go"))!.Cutoff.Label.ShouldBe("ChatGPT launch");
    }
}
