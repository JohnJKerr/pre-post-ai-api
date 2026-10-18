using Domain.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenFetchingTheContributionCalendar : IAsyncLifetime
{
    private readonly FakeGitHubApi _api = new GivenTheGitHubApi().Build();
    private GitHubActivity _activity = null!;

    public async Task InitializeAsync() => _activity = await GitHubClient.FetchFrom(_api);

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void Only_days_with_a_contribution_are_kept() =>
        _activity.ContributionDays.ShouldBe([new DateOnly(2025, 6, 2), new DateOnly(2026, 6, 8)]);

    [Fact]
    public void The_calendar_is_fetched_a_year_at_a_time() =>
        _api.Requests.Count(r => r.RequestUri!.AbsolutePath == "/graphql").ShouldBe(2);
}
