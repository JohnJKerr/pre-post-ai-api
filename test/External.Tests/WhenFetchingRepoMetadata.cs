using Domain;
using Domain.GitHub;
using External.GitHub;
using Shouldly;

namespace External.Tests;

public class WhenFetchingRepoMetadata : IAsyncLifetime
{
    private readonly FakeGitHubApi _api = new GivenTheGitHubApi().Build();
    private GitHubActivity _activity = null!;

    public async Task InitializeAsync()
    {
        var gitHub = new GitHubMetadata(new HttpClient(_api) { BaseAddress = new Uri("https://api.github.com") }, GitHubOptions.NoDelay);
        _activity = await gitHub.Fetch(new GitHubUser("octo-dev", "user-token"), new DateOnly(2024, 12, 29), new DateOnly(2026, 10, 17));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void Every_repo_the_user_can_see_is_listed() => _activity.Repos.Select(r => r.Name).ShouldBe(["ledger-sync", "side-project"]);

    [Fact]
    public void A_private_repo_is_marked_private() => RepoNamed("side-project").IsPrivate.ShouldBeTrue();

    [Fact]
    public void A_repo_s_creation_date_is_kept() => RepoNamed("side-project").CreatedOn.ShouldBe(new DateOnly(2026, 2, 1));

    [Fact]
    public void Only_the_user_s_weeks_are_kept_whatever_the_login_s_case() =>
        RepoNamed("ledger-sync").AuthorWeeks.ShouldBe([new Week(new DateOnly(2025, 6, 1), 300, 30, 3), new Week(new DateOnly(2025, 6, 8), 120, 10, 4)]);

    [Fact]
    public void Total_commits_count_every_author() => RepoNamed("ledger-sync").TotalCommits.ShouldBe(9);

    [Fact]
    public void The_punch_card_is_kept() => RepoNamed("ledger-sync").PunchCard.ShouldBe([new PunchCell(2, 10, 6), new PunchCell(3, 14, 3)]);

    [Fact]
    public void Language_bytes_are_kept() => RepoNamed("ledger-sync").LanguageBytes["SQL"].ShouldBe(10_000);

    [Fact]
    public void Requests_carry_the_user_s_token() =>
        _api.Requests.ShouldAllBe(r => r.Headers.Authorization!.ToString() == "Bearer user-token");

    private RepoMetadata RepoNamed(string name) => _activity.Repos.Single(r => r.Name == name);
}
