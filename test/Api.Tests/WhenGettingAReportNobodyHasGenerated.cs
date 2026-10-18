using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGettingAReportNobodyHasGenerated(ApiFactory factory)
{
    [Fact]
    public async Task It_is_not_found()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/reports/nobody-yet");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task The_message_says_nobody_has_generated_one()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/reports/nobody-yet");

        // Assert
        (await response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("nobody-yet hasn't generated a report yet.");
    }
}
