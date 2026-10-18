using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubSendsTheUserBack(ApiFactory factory) : IAsyncLifetime
{
    private HttpClient _browser = null!;
    private HttpResponseMessage _callback = null!;

    public async Task InitializeAsync()
    {
        _browser = factory.CreateBrowserSigningInWithGitHub();
        _callback = await _browser.SignInWithGitHub(returnUrl: "/r/octo-dev");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void They_return_to_where_they_started() => _callback.Headers.Location!.ToString().ShouldBe("/r/octo-dev");

    [Fact]
    public async Task They_are_signed_in_as_their_GitHub_login() =>
        (await _browser.GetFromJsonAsync<SessionResponse>("/api/session"))!.Handle.ShouldBe("octo-dev");
}
