using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportFromThisWeek(ApiFactory factory)
{
    [Fact]
    public async Task It_is_unprocessable()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("eager-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { cutoff = "2026-10-18" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task The_message_says_a_full_week_must_pass()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("eager-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { cutoff = "2026-10-18" });

        // Assert
        (await response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message
            .ShouldBe("Pick a cut-off at least one full week ago, so there's a week to compare.");
    }
}
