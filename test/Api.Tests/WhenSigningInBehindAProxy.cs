using System.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSigningInBehindAProxy(ApiFactory factory)
{
    [Fact]
    public async Task GitHub_is_asked_to_return_to_the_public_https_address()
    {
        // Arrange
        var proxy = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, BaseAddress = new Uri("http://localhost") });
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/sign-in");
        request.Headers.Add("X-Forwarded-Proto", "https");
        request.Headers.Add("X-Forwarded-Host", "shift-api.example");

        // Act
        var response = await proxy.SendAsync(request);

        // Assert
        HttpUtility.ParseQueryString(response.Headers.Location!.Query)["redirect_uri"].ShouldBe("https://shift-api.example/api/auth/callback");
    }
}
