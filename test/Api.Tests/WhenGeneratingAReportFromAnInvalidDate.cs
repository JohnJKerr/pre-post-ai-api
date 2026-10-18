using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportFromAnInvalidDate(ApiFactory factory)
{
    [Fact]
    public async Task It_is_unprocessable()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("typo-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { cutoff = "2025-02-30" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task The_message_explains_the_date_format()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("typo-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { cutoff = "2025-02-30" });

        // Assert
        (await response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("2025-02-30 isn't a valid date. Use YYYY-MM-DD.");
    }
}
