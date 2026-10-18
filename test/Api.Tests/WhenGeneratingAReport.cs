using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReport(ApiFactory factory)
{
    [Fact]
    public async Task It_is_created()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("new-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task It_is_located_at_the_user_s_report()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("new-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        response.Headers.Location!.ToString().ShouldBe("/api/reports/new-dev");
    }

    [Fact]
    public async Task It_is_for_the_signed_in_user()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("new-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await response.Content.ReadFromJsonAsync<ReportResponse>())!.Handle.ShouldBe("new-dev");
    }

    [Fact]
    public async Task It_compares_either_side_of_Claude_Opus_4_5_by_default()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("new-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await response.Content.ReadFromJsonAsync<ReportResponse>())!.Cutoff.ShouldBe(new CutoffResponse("Claude Opus 4.5", "2025-11-24"));
    }

    [Fact]
    public async Task It_is_built_from_the_user_s_GitHub_activity()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("new-dev"));

        // Act
        var response = await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await response.Content.ReadFromJsonAsync<ReportResponse>())!.Post.Added.ShouldBe(2_000);
    }

    [Fact]
    public async Task It_is_saved_for_anyone_to_view()
    {
        // Arrange
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("saved-dev"));

        // Act
        await client.PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await factory.CreateClient().GetAsync("/api/reports/saved-dev")).StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
