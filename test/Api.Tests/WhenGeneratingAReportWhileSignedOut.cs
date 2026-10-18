using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportWhileSignedOut(ApiFactory factory)
{
    [Fact]
    public async Task It_is_unauthorised()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedOut());

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task The_message_asks_them_to_sign_in()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedOut());

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("Sign in with GitHub to generate your report.");
    }
}
