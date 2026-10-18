using System.Net;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenTheApiRestarts(ApiFactory factory)
{
    [Fact]
    public async Task Signed_in_users_stay_signed_in()
    {
        // Arrange
        var sessionCookie = await SignInAndKeepTheSessionCookie();

        // Act
        var response = await factory.CreateBrowserSigningInWithGitHub().SendAsync(ASessionCheckWith(sessionCookie));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Sessions_are_only_readable_with_the_keys_in_its_database()
    {
        // Arrange
        var sessionCookie = await SignInAndKeepTheSessionCookie();

        // Act
        var response = await factory.CreateBrowserSigningInWithGitHub(database: "another-deployment").SendAsync(ASessionCheckWith(sessionCookie));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private async Task<string> SignInAndKeepTheSessionCookie()
    {
        var callback = await factory.CreateBrowserSigningInWithGitHub().SignInWithGitHub();
        return callback.Headers.GetValues("Set-Cookie").Single(c => c.StartsWith("pre-post-session=")).Split(';')[0];
    }

    private static HttpRequestMessage ASessionCheckWith(string sessionCookie)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/session");
        request.Headers.Add("Cookie", sessionCookie);
        return request;
    }
}
