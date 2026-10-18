using Domain.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenTheUserHasMoreReposThanFitOnAPage : IAsyncLifetime
{
    private GitHubActivity _activity = null!;

    public async Task InitializeAsync() => _activity = await GitHubClient.FetchFrom(new GivenTheGitHubApi().WithASecondPageOfRepos().Build());

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void Repos_on_every_page_are_listed() =>
        _activity.Repos.Select(r => r.Name).ShouldBe(["ledger-sync", "side-project", "old-tool"]);
}
