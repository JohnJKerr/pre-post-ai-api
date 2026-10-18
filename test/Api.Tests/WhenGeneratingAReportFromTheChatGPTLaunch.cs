using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportFromTheChatGPTLaunch(ApiFactory factory)
{
    [Fact]
    public async Task The_cutoff_is_named_ChatGPT_launch()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("early-adopter"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { cutoff = "2022-11-30" });

        // Assert
        (await response.Content.ReadFromJsonAsync<ReportResponse>())!.Cutoff.ShouldBe(new CutoffResponse("ChatGPT launch", "2022-11-30"));
    }

    [Fact]
    public async Task Each_side_spans_every_complete_week_since()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("early-adopter"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { cutoff = "2022-11-30" });

        // Assert
        (await response.Content.ReadFromJsonAsync<ReportResponse>())!.Weeks.ShouldBe(203);
    }
}
