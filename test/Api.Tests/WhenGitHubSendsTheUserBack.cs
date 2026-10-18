using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubSendsTheUserBack(ApiFactory factory)
{
    [Fact]
    public async Task They_return_to_where_they_started()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();

        // Act
        var callback = await browser.SignInWithGitHub(returnUrl: "/r/octo-dev");

        // Assert
        callback.Headers.Location!.ToString().ShouldBe("/r/octo-dev");
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
