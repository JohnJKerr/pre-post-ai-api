using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenCountingReports(ApiFactory factory)
{
    [Fact]
    public async Task It_succeeds()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/report-count");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task A_new_person_generating_a_report_adds_one()
    {
        // Arrange
        var before = await TheCount();

        // Act
        await GenerateAReportAs("first-timer", 11_001);

        // Assert
        (await TheCount()).ShouldBe(before + 1);
    }

    private async Task GenerateAReportAs(string login, long gitHubId)
    {
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs(login, gitHubId));
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub());
    }

    private async Task<long> TheCount() =>
        (await factory.CreateClient().GetFromJsonAsync<ReportCountResponse>("/api/report-count"))!.Count;
}
