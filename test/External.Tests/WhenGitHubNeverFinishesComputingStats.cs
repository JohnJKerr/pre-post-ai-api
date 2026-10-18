using Domain.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenGitHubNeverFinishesComputingStats
{
    [Fact]
    public async Task Fetching_gives_up_saying_the_stats_are_pending() =>
        await Should.ThrowAsync<GitHubStatsPendingException>(() =>
            GitHubClient.FetchFrom(new GivenTheGitHubApi().WithStatsNeverReady("ledger-sync").Build()));
}
