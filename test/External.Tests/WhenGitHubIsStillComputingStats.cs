using Domain.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenGitHubIsStillComputingStats : IAsyncLifetime
{
    private GitHubActivity _activity = null!;

    public async Task InitializeAsync() =>
        _activity = await GitHubClient.FetchFrom(new GivenTheGitHubApi().WithStatsStillComputing("ledger-sync", times: 2).Build());

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void The_stats_are_fetched_once_ready() => _activity.Repos.Single(r => r.Name == "ledger-sync").AuthorWeeks.Count.ShouldBe(2);
}
