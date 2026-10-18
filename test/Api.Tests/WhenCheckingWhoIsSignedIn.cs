using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenCheckingWhoIsSignedIn(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() => _response = await factory.CreateClientFor(FakeApiUser.SignedInAs("octo-dev")).GetAsync("/api/session");

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_succeeds() => _response.StatusCode.ShouldBe(HttpStatusCode.OK);

    [Fact]
    public async Task It_names_the_signed_in_user() =>
        (await _response.Content.ReadFromJsonAsync<SessionResponse>())!.Handle.ShouldBe("octo-dev");
}
