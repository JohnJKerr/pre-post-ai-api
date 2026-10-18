using System.Net;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenTheFrontEndCallsTheApi(ApiFactory factory)
{
    [Fact]
    public async Task The_front_end_may_read_the_response()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/session");
        request.Headers.Add("Origin", ApiFactory.FrontEndOrigin);

        // Act
        var response = await factory.CreateClient().SendAsync(request);

        // Assert
        response.Headers.GetValues("Access-Control-Allow-Origin").ShouldBe([ApiFactory.FrontEndOrigin]);
    }

    [Fact]
    public async Task The_front_end_may_send_the_session_cookie()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/session");
        request.Headers.Add("Origin", ApiFactory.FrontEndOrigin);

        // Act
        var response = await factory.CreateClient().SendAsync(request);

        // Assert
        response.Headers.GetValues("Access-Control-Allow-Credentials").ShouldBe(["true"]);
    }

    [Fact]
    public async Task Another_site_may_not_read_the_response()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/session");
        request.Headers.Add("Origin", "https://other.example");

        // Act
        var response = await factory.CreateClient().SendAsync(request);

        // Assert
        response.Headers.Contains("Access-Control-Allow-Origin").ShouldBeFalse();
    }

    [Fact]
    public async Task A_preflight_for_generating_a_report_has_no_content()
    {
        // Arrange
        var preflight = APreflightForGeneratingAReport();

        // Act
        var response = await factory.CreateClient().SendAsync(preflight);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task A_preflight_for_generating_a_report_allows_POST()
    {
        // Arrange
        var preflight = APreflightForGeneratingAReport();

        // Act
        var response = await factory.CreateClient().SendAsync(preflight);

        // Assert
        response.Headers.GetValues("Access-Control-Allow-Methods").ShouldContain(m => m.Contains("POST"));
    }

    [Fact]
    public async Task A_preflight_for_generating_a_report_allows_a_JSON_body()
    {
        // Arrange
        var preflight = APreflightForGeneratingAReport();

        // Act
        var response = await factory.CreateClient().SendAsync(preflight);

        // Assert
        response.Headers.GetValues("Access-Control-Allow-Headers").ShouldContain(h => h.Contains("Content-Type", StringComparison.OrdinalIgnoreCase));
    }

    private static HttpRequestMessage APreflightForGeneratingAReport()
    {
        var preflight = new HttpRequestMessage(HttpMethod.Options, "/api/reports");
        preflight.Headers.Add("Origin", ApiFactory.FrontEndOrigin);
        preflight.Headers.Add("Access-Control-Request-Method", "POST");
        preflight.Headers.Add("Access-Control-Request-Headers", "content-type");
        return preflight;
    }
}
