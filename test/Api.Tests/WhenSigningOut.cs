using System.Net;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSigningOut(ApiFactory factory)
{
    [Fact]
    public async Task It_succeeds_with_nothing_to_say()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();
        await browser.SignInWithGitHub();

        // Act
        var response = await browser.PostAsync("/api/auth/sign-out", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task They_are_no_longer_signed_in()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub();
        await browser.SignInWithGitHub();

        // Act
        await browser.PostAsync("/api/auth/sign-out", null);

        // Assert
        (await browser.GetAsync("/api/session")).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
