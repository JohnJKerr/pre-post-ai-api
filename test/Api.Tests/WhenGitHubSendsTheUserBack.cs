using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubSendsTheUserBack(ApiFactory factory)
{
    [Fact]
    public async Task They_return_to_the_front_end_page_they_started_from()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();

        // Act
        var callback = await browser.SignInWithGitHub(returnUrl: "/generate");

        // Assert
        callback.Headers.Location!.ToString().ShouldBe($"{ApiFactory.FrontEndOrigin}/generate");
    }

    [Fact]
    public async Task A_return_url_on_another_site_takes_them_to_the_front_end_s_home_page()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();

        // Act
        var callback = await browser.SignInWithGitHub(returnUrl: "https://evil.example");

        // Assert
        callback.Headers.Location!.ToString().ShouldBe($"{ApiFactory.FrontEndOrigin}/");
    }

    [Fact]
    public async Task They_are_signed_in_as_their_GitHub_login()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();

        // Act
        await browser.SignInWithGitHub();

        // Assert
        (await browser.GetFromJsonAsync<SessionResponse>("/api/session"))!.Handle.ShouldBe("octo-dev");
    }
}
