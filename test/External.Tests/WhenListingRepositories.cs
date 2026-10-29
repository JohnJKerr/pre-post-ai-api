using Shouldly;

namespace External.Tests;

public class WhenListingRepositories
{
    [Fact]
    public async Task The_list_is_ordered_by_most_recent_push()
    {
        // Arrange
        var api = new GivenTheGitHubApi().Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Requests.ShouldContain(r => r.RequestUri!.PathAndQuery == "/user/repos?per_page=100&sort=pushed&direction=desc");
    }
}
