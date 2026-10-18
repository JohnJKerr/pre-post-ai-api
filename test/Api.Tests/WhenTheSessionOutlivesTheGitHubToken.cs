using System.Net;
using Microsoft.Extensions.Time.Testing;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenTheSessionOutlivesTheGitHubToken(ApiFactory factory)
{
    [Fact]
    public async Task They_stay_signed_in_while_the_token_lasts()
    {
        // Arrange
        var clock = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var browser = factory.CreateBrowserSigningInWithGitHub(clock: clock);
        await browser.SignInWithGitHub();
        clock.Advance(TimeSpan.FromHours(7));

        // Act
        var response = await browser.GetAsync("/api/session");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task They_are_signed_out_when_the_token_expires()
    {
        // Arrange
        var clock = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var browser = factory.CreateBrowserSigningInWithGitHub(clock: clock);
        await browser.SignInWithGitHub();
        clock.Advance(TimeSpan.FromHours(7));
        await browser.GetAsync("/api/session");
        clock.Advance(TimeSpan.FromHours(1.5));

        // Act
        var response = await browser.GetAsync("/api/session");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
