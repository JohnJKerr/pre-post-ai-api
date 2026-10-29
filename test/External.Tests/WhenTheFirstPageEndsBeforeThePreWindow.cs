using Shouldly;

namespace External.Tests;

public class WhenTheFirstPageEndsBeforeThePreWindow
{
    [Fact]
    public async Task The_next_page_is_not_requested()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithAPageThatEndsBeforeThePreWindow().Build();

        // Act
        await GitHubClient.FetchFrom(api);

        // Assert
        api.Requests.ShouldNotContain(r => r.RequestUri!.Query.Contains("page=2"));
    }
}
