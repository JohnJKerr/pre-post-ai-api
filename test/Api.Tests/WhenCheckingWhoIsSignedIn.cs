using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenCheckingWhoIsSignedIn(ApiFactory factory)
{
    [Fact]
    public async Task It_succeeds()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("octo-dev"));

        // Act
        var response = await client.GetAsync("/api/session");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task It_names_the_signed_in_user()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("octo-dev"));

        // Act
        var session = await client.GetFromJsonAsync<SessionResponse>("/api/session");

        // Assert
        session!.Handle.ShouldBe("octo-dev");
    }
}
