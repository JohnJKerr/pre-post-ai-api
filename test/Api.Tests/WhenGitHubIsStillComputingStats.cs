using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubIsStillComputingStats(ApiFactory factory)
{
    [Fact]
    public async Task The_service_is_unavailable_for_now()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("busy-dev"), FakeGitHub.StillComputingStats());

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task The_message_asks_them_to_try_again_shortly()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("busy-dev"), FakeGitHub.StillComputingStats());

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message
            .ShouldBe("GitHub is still crunching your repository stats. Try again in a minute.");
    }
}
