using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGitHubSendsTheUserBack(ApiFactory factory)
{
    [Fact]
    public async Task They_are_sent_to_watch_their_report_being_generated()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("returning-dev", 10_001));

        // Act
        var callback = await browser.SignInWithGitHub();

        // Assert
        callback.Headers.Location!.ToString().ShouldStartWith($"{ApiFactory.FrontEndOrigin}/generating?id=");
    }

    [Fact]
    public async Task They_are_not_given_a_session_only_the_sign_in_cookie_is_cleared()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("returning-dev", 10_001));

        // Act
        var callback = await browser.SignInWithGitHub();

        // Assert
        callback.Headers.GetValues("Set-Cookie").ShouldAllBe(cookie => cookie.Contains("expires=Thu, 01 Jan 1970"));
    }

    [Fact]
    public async Task Their_report_is_generated_in_the_background()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("background-dev", 10_002));
        var callback = await browser.SignInWithGitHub();

        // Act
        var generation = await browser.WaitForTheGeneration(callback);

        // Assert
        generation.Status.ShouldBe("ready");
    }

    [Fact]
    public async Task Their_report_is_saved_for_anyone_to_view()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("viewable-dev", 10_003));
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub());

        // Act
        var response = await factory.CreateClient().GetAsync("/api/reports/viewable-dev");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Their_report_is_built_from_their_GitHub_activity()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("active-dev", 10_004));
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub());

        // Act
        var report = await factory.CreateClient().GetFromJsonAsync<ReportResponse>("/api/reports/active-dev");

        // Assert
        report!.Post.Added.ShouldBe(2_000);
    }

    [Fact]
    public async Task Their_report_compares_either_side_of_24_November_2025_by_default()
    {
        // Arrange
        var browser = factory.CreateBrowserSigningInWithGitHub(gitHubOAuth: FakeGitHubOAuth.SigningInAs("default-dev", 10_005));
        await browser.WaitForTheGeneration(await browser.SignInWithGitHub());

        // Act
        var report = await factory.CreateClient().GetFromJsonAsync<ReportResponse>("/api/reports/default-dev");

        // Assert
        report!.Cutoff.ShouldBe(new CutoffResponse("2025-11-24"));
    }
}
