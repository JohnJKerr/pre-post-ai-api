using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenTheFrontEndCallsTheApi(ApiFactory factory)
{
    [Fact]
    public async Task The_front_end_may_read_the_response()
    {
        // Arrange
        var request = ARequestFrom(ApiFactory.FrontEndOrigin);

        // Act
        var response = await factory.CreateClient().SendAsync(request);

        // Assert
        response.Headers.GetValues("Access-Control-Allow-Origin").ShouldBe([ApiFactory.FrontEndOrigin]);
    }

    [Fact]
    public async Task No_cookies_are_accepted()
    {
        // Arrange
        var request = ARequestFrom(ApiFactory.FrontEndOrigin);

        // Act
        var response = await factory.CreateClient().SendAsync(request);

        // Assert
        response.Headers.Contains("Access-Control-Allow-Credentials").ShouldBeFalse();
    }

    [Fact]
    public async Task Another_site_may_not_read_the_response()
    {
        // Arrange
        var request = ARequestFrom("https://other.example");

        // Act
        var response = await factory.CreateClient().SendAsync(request);

        // Assert
        response.Headers.Contains("Access-Control-Allow-Origin").ShouldBeFalse();
    }

    private static HttpRequestMessage ARequestFrom(string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/sample-report");
        request.Headers.Add("Origin", origin);
        return request;
    }
}
