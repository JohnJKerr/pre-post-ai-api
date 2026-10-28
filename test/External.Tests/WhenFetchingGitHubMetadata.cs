using External.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenFetchingGitHubMetadata
{
    [Fact]
    public async Task The_fetch_logs_request_counts_and_phase_durations()
    {
        // Arrange
        var api = new GivenTheGitHubApi().WithStatsStillComputing("ledger-sync", times: 1).Build();
        var log = new GivenAGitHubLog<GitHubMetadata>();
        var metadata = new GitHubMetadata(
            new HttpClient(api) { BaseAddress = new Uri("https://api.github.com") },
            GitHubOptions.NoDelay,
            TimeProvider.System,
            log);

        // Act
        await metadata.Fetch(GitHubClient.User, GitHubClient.Windows);

        // Assert
        log.Entries.ShouldContain(entry =>
            entry.Any(p => p.Key == "RecentRepoCount" && Equals(p.Value, 2)) &&
            entry.Any(p => p.Key == "StatsPendingResponses" && Equals(p.Value, 1)) &&
            entry.Any(p => p.Key == "ListingMs") &&
            entry.Any(p => p.Key == "StatsMs") &&
            entry.Any(p => p.Key == "LanguagesMs") &&
            entry.Any(p => p.Key == "CalendarsMs"));
    }
}
