using System.Net;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSigningOut(ApiFactory factory) : IAsyncLifetime
{
    private HttpClient _browser = null!;
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync()
    {
        _browser = factory.CreateBrowserSigningInWithGitHub();
        await _browser.SignInWithGitHub();
        _response = await _browser.PostAsync("/api/auth/sign-out", null);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_succeeds_with_nothing_to_say() => _response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

    [Fact]
    public async Task They_are_no_longer_signed_in() => (await _browser.GetAsync("/api/session")).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
}
