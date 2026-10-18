using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubIsStillComputingStats(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() =>
        _response = await factory.CreateClientFor(FakeApiUser.SignedInAs("busy-dev"), FakeGitHub.StillComputingStats()).PostAsJsonAsync("/api/reports", new { });

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void The_service_is_unavailable_for_now() => _response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);

    [Fact]
    public async Task The_message_asks_them_to_try_again_shortly() =>
        (await _response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("GitHub is still crunching your repository stats. Try again in a minute.");
}
