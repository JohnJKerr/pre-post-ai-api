using Shouldly;
using static Domain.Tests.GivenGitHubActivity;

namespace Domain.Tests;

public class WhenChoosingPunchCardRepos
{
    private readonly Profile _profile = new GivenGitHubActivity()
        .WithRepo(new GivenAGitHubRepo("solo")
            .WithAuthorWeek(PostWeek, added: 900, commits: 9)
            .WithCommitsByOthers(1)
            .WithPunchCell(day: 2, hour: 21, commits: 6)
            .WithPunchCell(day: 3, hour: 22, commits: 4))
        .WithRepo(new GivenAGitHubRepo("shared")
            .WithAuthorWeek(PostWeek, added: 800, commits: 8)
            .WithCommitsByOthers(2)
            .WithPunchCell(day: 1, hour: 10, commits: 10))
        .WithRepo(new GivenAGitHubRepo("spanning")
            .WithAuthorWeek(PreWeek, added: 300, commits: 3)
            .WithAuthorWeek(PostWeek, added: 100, commits: 1)
            .WithPunchCell(day: 0, hour: 0, commits: 4))
        .WithRepo(new GivenAGitHubRepo("abandoned")
            .WithAuthorWeek(BeforeBothWindows, added: 500, commits: 5)
            .WithPunchCell(day: 4, hour: 4, commits: 5))
        .Assemble();

    [Fact]
    public void A_repo_the_author_wrote_90_percent_of_after_the_cutoff_is_eligible_after_it() =>
        RepoNamed("solo").PunchCardEra.ShouldBe(Era.Post);

    [Fact]
    public void A_repo_others_wrote_more_than_a_tenth_of_is_not_eligible() => RepoNamed("shared").PunchCardEra.ShouldBeNull();

    [Fact]
    public void A_repo_with_activity_either_side_of_the_cutoff_is_not_eligible() =>
        RepoNamed("spanning").PunchCardEra.ShouldBeNull();

    [Fact]
    public void A_repo_with_no_commits_in_either_window_is_not_eligible() =>
        RepoNamed("abandoned").PunchCardEra.ShouldBeNull();

    [Fact]
    public void The_window_s_punch_card_sums_only_eligible_repos() =>
        _profile.Activity.Post.PunchCard.Sum(c => c.Commits).ShouldBe(10);

    [Fact]
    public void The_punch_card_has_every_hour_of_the_week() => _profile.Activity.Post.PunchCard.Count.ShouldBe(168);

    [Fact]
    public void Punch_share_is_the_eligible_repos_share_of_the_window_s_commits() =>
        _profile.Activity.Post.PunchShare.ShouldBe(9 / 18d);

    private Repo RepoNamed(string name) => _profile.Repos.Single(r => r.Name == name);
}
