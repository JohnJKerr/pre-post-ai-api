using System.Net.Http.Json;
using Api.Models;
using Microsoft.Extensions.Time.Testing;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenSomeoneTakesARenamedUsersOldHandle(ApiFactory factory)
{
    [Fact]
    public async Task The_handle_shows_the_new_owner_s_report()
    {
        // Arrange
        await factory.CreateClientFor(FakeApiUser.SignedInAs("passed-on", gitHubId: 8_001)).PostAsJsonAsync("/api/reports", new { });

        var aDayLater = new FakeTimeProvider(ApiFactory.Now.AddDays(1));

        // Act
        await factory.CreateClientFor(FakeApiUser.SignedInAs("passed-on", gitHubId: 8_002), clock: aDayLater)
            .PostAsJsonAsync("/api/reports", new { cutoff = "2022-11-30" });

        // Assert
        (await factory.CreateClient().GetFromJsonAsync<ReportResponse>("/api/reports/passed-on"))!.Cutoff.Label.ShouldBe("ChatGPT launch");
    }
}
