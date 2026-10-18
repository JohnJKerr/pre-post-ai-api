using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenCheckingWhoIsSignedInWhileSignedOut(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() => _response = await factory.CreateClientFor(FakeApiUser.SignedOut()).GetAsync("/api/session");

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_is_unauthorised() => _response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

    [Fact]
    public async Task The_message_says_nobody_is_signed_in() =>
        (await _response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("You're not signed in.");
}
