using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportWithNoCommitsInEitherWindow(ApiFactory factory)
{
    [Fact]
    public async Task There_is_no_content()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("quiet-dev"), FakeGitHub.WithNoCommits());

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Nothing_is_saved()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("silent-dev"), FakeGitHub.WithNoCommits());

        // Act
        await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await factory.CreateClient().GetAsync("/api/reports/silent-dev")).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
