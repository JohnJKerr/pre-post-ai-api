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
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("Second-Go"));
        await client.PostAsJsonAsync("/api/reports", new { });

        // Act
        await client.PostAsJsonAsync("/api/reports", new { cutoff = "2022-11-30" });

        // Assert
        (await client.GetFromJsonAsync<ReportResponse>("/api/reports/second-go"))!.Cutoff.Label.ShouldBe("ChatGPT launch");
    }
}
