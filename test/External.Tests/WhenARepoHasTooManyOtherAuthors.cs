using Shouldly;

namespace External.Tests;

public class WhenARepoHasTooManyOtherAuthors
{
    [Fact]
    public async Task Its_punch_card_is_not_requested()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Requests.ShouldNotContain(r => r.RequestUri!.AbsolutePath == "/repos/octo-dev/ledger-sync/stats/punch_card");
    }
}
