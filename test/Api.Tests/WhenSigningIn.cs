using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSigningIn(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync()
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost") });
        _response = await client.GetAsync("/api/auth/sign-in");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void The_user_is_sent_to_GitHub_to_authorise_the_app() =>
        _response.Headers.Location!.GetLeftPart(UriPartial.Path).ShouldBe("https://github.com/login/oauth/authorize");

    [Fact]
    public void GitHub_is_told_which_app_is_asking() => _response.Headers.Location!.Query.ShouldContain("client_id=test-client-id");

    [Fact]
    public void It_is_a_redirect() => _response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
}
