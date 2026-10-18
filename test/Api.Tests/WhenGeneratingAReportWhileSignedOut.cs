using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportWhileSignedOut(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() =>
        _response = await factory.CreateClientFor(FakeApiUser.SignedOut()).PostAsJsonAsync("/api/reports", new { });

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_is_unauthorised() => _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

    [Fact]
    public async Task The_message_asks_them_to_sign_in() =>
        (await _response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("Sign in with GitHub to generate your report.");
}
