using System.Net;
using System.Net.Http.Json;
using System.Text;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenAnotherSitePostsToTheApi(ApiFactory factory)
{
    [Fact]
    public async Task Generating_a_report_is_forbidden()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("octo-dev"));
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/reports") { Content = JsonContent.Create(new { }) };

        // Act
        var response = await client.SendAsync(WithOrigin(request, "https://other.developkerr.com"));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Signing_out_is_forbidden()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();
        await browser.SignInWithGitHub();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/sign-out");

        // Act
        var response = await browser.SendAsync(WithOrigin(request, "https://other.developkerr.com"));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task A_post_without_an_origin_is_forbidden()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("octo-dev"));
        client.DefaultRequestHeaders.Remove("Origin");

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task A_report_request_that_isn_t_JSON_is_refused()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("octo-dev"));

        // Act
        var response = await client.PostAsync("/api/reports", new StringContent("cutoff=2025-11-24", Encoding.UTF8, "text/plain"));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.UnsupportedMediaType);
    }

    private static HttpRequestMessage WithOrigin(HttpRequestMessage request, string origin)
    {
        request.Headers.Remove("Origin");
        request.Headers.Add("Origin", origin);
        return request;
    }
}
