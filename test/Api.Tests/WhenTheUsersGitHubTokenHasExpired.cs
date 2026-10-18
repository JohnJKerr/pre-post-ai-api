using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenTheUsersGitHubTokenHasExpired(ApiFactory factory)
{
    [Fact]
    public async Task Generating_a_report_is_unauthorised()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("lapsed-dev"), FakeGitHub.RejectingTheToken());

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task The_message_asks_them_to_sign_in_again()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("lapsed-dev"), FakeGitHub.RejectingTheToken());

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("Your GitHub sign-in has expired. Sign in again.");
    }

    [Fact]
    public async Task Their_session_ends()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(FakeGitHub.RejectingTheToken());
        await browser.SignInWithGitHub();

        // Act
        await browser.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await browser.GetAsync("/api/session")).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
