using System.Net.Http.Json;
using System.Web;
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
        var signIn = await _browser.GetAsync("/api/auth/sign-in?returnUrl=/r/octo-dev");
        var state = HttpUtility.ParseQueryString(signIn.Headers.Location!.Query)["state"];
        _callback = await _browser.GetAsync($"/api/auth/callback?code=github-code&state={Uri.EscapeDataString(state!)}");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void They_return_to_where_they_started() => _callback.Headers.Location!.ToString().ShouldBe("/r/octo-dev");

    [Fact]
    public async Task They_are_signed_in_as_their_GitHub_login() =>
        (await _browser.GetFromJsonAsync<SessionResponse>("/api/session"))!.Handle.ShouldBe("octo-dev");
}
