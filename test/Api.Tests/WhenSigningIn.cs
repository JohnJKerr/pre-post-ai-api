using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSigningIn(ApiFactory factory)
{
    [Fact]
    public async Task It_is_a_redirect()
    {
        // Arrange
        var browser = ABrowser();

        // Act
        var response = await browser.GetAsync("/api/auth/sign-in");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task The_user_is_sent_to_GitHub_to_authorise_the_app()
    {
        // Arrange
        var browser = ABrowser();

        // Act
        var response = await browser.GetAsync("/api/auth/sign-in");

        // Assert
        response.Headers.Location!.GetLeftPart(UriPartial.Path).ShouldBe("https://github.com/login/oauth/authorize");
    }

    [Fact]
    public async Task GitHub_is_told_which_app_is_asking()
    {
        // Arrange
        var browser = ABrowser();

        // Act
        var response = await browser.GetAsync("/api/auth/sign-in");

        // Assert
        response.Headers.Location!.Query.ShouldContain("client_id=test-client-id");
    }

    private HttpClient ABrowser() =>
        factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("https://localhost") });
}
