using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenAUserRenamesTheirGitHubAccount(ApiFactory factory)
{
    private const long GitHubId = 7_001;

    [Fact]
    public async Task Their_new_handle_shows_their_report()
    {
        // Arrange
        await factory.CreateClientFor(FakeApiUser.SignedInAs("before-rename", GitHubId)).PostAsJsonAsync("/api/reports", new { });

        // Act
        await factory.CreateClientFor(FakeApiUser.SignedInAs("after-rename", GitHubId)).PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await factory.CreateClient().GetAsync("/api/reports/after-rename")).StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Their_old_handle_no_longer_has_a_report()
    {
        // Arrange
        await factory.CreateClientFor(FakeApiUser.SignedInAs("was-renamed", GitHubId + 1)).PostAsJsonAsync("/api/reports", new { });

        // Act
        await factory.CreateClientFor(FakeApiUser.SignedInAs("is-renamed", GitHubId + 1)).PostAsJsonAsync("/api/reports", new { });

        // Assert
        (await factory.CreateClient().GetAsync("/api/reports/was-renamed")).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
