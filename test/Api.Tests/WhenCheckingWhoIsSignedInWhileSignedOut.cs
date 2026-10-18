using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenCheckingWhoIsSignedInWhileSignedOut(ApiFactory factory)
{
    [Fact]
    public async Task It_is_unauthorised()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedOut());

        // Act
        var response = await client.GetAsync("/api/session");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task The_message_says_nobody_is_signed_in()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedOut());

        // Act
        var response = await client.GetAsync("/api/session");

        // Assert
        (await response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("You're not signed in.");
    }
}
