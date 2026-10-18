using System.Net;
using System.Text.Json.Nodes;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGettingTheSampleReport(ApiFactory factory)
{
    [Fact]
    public async Task The_sample_report_is_returned()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/sample-report");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task The_report_matches_the_front_end_contract()
    {
        // Arrange
        var client = factory.CreateClient();
        var expected = JsonNode.Parse(await File.ReadAllTextAsync("Fixtures/sample-report.json"));

        // Act
        var response = await client.GetAsync("/api/sample-report");

        // Assert
        var actual = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        JsonDifference.Between(expected, actual).ShouldBeNull();
    }
}
