using Domain.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenARepoIsEmpty : IAsyncLifetime
{
    private RepoMetadata _repo = null!;

    public async Task InitializeAsync() =>
        _repo = (await GitHubClient.FetchFrom(new GivenTheGitHubApi().WithEmptyRepo("side-project").Build())).Repos.Single(r => r.Name == "side-project");

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_has_no_weeks() => _repo.AuthorWeeks.ShouldBeEmpty();

    [Fact]
    public void It_has_no_punch_card() => _repo.PunchCard.ShouldBeEmpty();
}
