using Shouldly;
using static Domain.Tests.GivenGitHubActivity;

namespace Domain.Tests;

public class WhenAssemblingAProfile
{
    private readonly Profile _profile = new GivenGitHubActivity()
        .WithRepo(new GivenAGitHubRepo("legacy")
            .Writing("C#", 3_000)
            .Writing("SQL", 1_000)
            .WithAuthorWeek(BeforeBothWindows, added: 9_999, commits: 99)
            .WithAuthorWeek(PreWeek, added: 300, commits: 3)
            .WithAuthorWeek(PostWeek, added: 100, commits: 1))
        .WithRepo(new GivenAGitHubRepo("side-project")
            .Private()
            .CreatedOn(new DateOnly(2026, 2, 1))
            .WithAuthorWeek(PostWeek, added: 300, commits: 5))
        .WithContributionsOn(new DateOnly(2025, 6, 2), new DateOnly(2026, 6, 8), new DateOnly(2026, 6, 9), BeforeBothWindows)
        .Assemble();

    [Fact]
    public void It_is_for_the_signed_in_user() => _profile.Handle.ShouldBe("octo-dev");

    [Fact]
    public void It_was_generated_now() => _profile.GeneratedAt.ShouldBe(Now);

    [Fact]
    public void Every_complete_week_of_the_window_is_kept() => _profile.Activity.Pre.Weeks.Count.ShouldBe(47);

    [Fact]
    public void Weeks_start_at_the_beginning_of_the_window() =>
        _profile.Activity.Pre.Weeks[0].Start.ShouldBe(new DateOnly(2024, 12, 29));

    [Fact]
    public void A_week_s_lines_are_summed_across_repos() =>
        _profile.Activity.Post.Weeks.Single(w => w.Start == PostWeek).Added.ShouldBe(400);

    [Fact]
    public void Activity_outside_both_windows_is_left_out() => _profile.Activity.Pre.Weeks.Sum(w => w.Commits).ShouldBe(3);

    [Fact]
    public void A_repo_s_share_is_its_fraction_of_the_window_s_lines() =>
        _profile.Repos.Single(r => r.Name == "legacy").Share.ShouldBe(new PerEra<double>(Pre: 1, Post: 0.25));

    [Fact]
    public void A_repo_created_in_a_window_was_started_in_it() =>
        _profile.Repos.Single(r => r.Name == "side-project").StartedIn.ShouldBe(Era.Post);

    [Fact]
    public void A_repo_created_before_both_windows_was_not_started_in_either() =>
        _profile.Repos.Single(r => r.Name == "legacy").StartedIn.ShouldBeNull();

    [Fact]
    public void Languages_are_shares_of_the_repo_s_bytes() =>
        _profile.Repos.Single(r => r.Name == "legacy").Languages["C#"].ShouldBe(0.75);

    [Fact]
    public void A_private_repo_stays_private() => _profile.Repos.Single(r => r.Name == "side-project").IsPrivate.ShouldBeTrue();

    [Fact]
    public void Active_days_are_counted_within_the_window() => _profile.Activity.Post.ActiveDays.ShouldBe(2);
}
