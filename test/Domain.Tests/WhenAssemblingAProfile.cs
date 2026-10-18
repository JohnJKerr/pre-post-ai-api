using Shouldly;
using static Domain.Tests.GivenGitHubActivity;

namespace Domain.Tests;

public class WhenAssemblingAProfile
{
    [Fact]
    public void It_is_for_the_signed_in_user()
    {
        // Arrange
        var activity = new GivenGitHubActivity();

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Handle.ShouldBe("octo-dev");
    }

    [Fact]
    public void It_belongs_to_the_user_s_GitHub_account()
    {
        // Arrange
        var activity = new GivenGitHubActivity();

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.GitHubId.ShouldBe(GitHubId);
    }

    [Fact]
    public void It_was_generated_now()
    {
        // Arrange
        var activity = new GivenGitHubActivity();

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.GeneratedAt.ShouldBe(Now);
    }

    [Fact]
    public void Every_complete_week_of_the_window_is_kept()
    {
        // Arrange
        var activity = new GivenGitHubActivity();

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Pre.Weeks.Count.ShouldBe(47);
    }

    [Fact]
    public void Weeks_start_at_the_beginning_of_the_window()
    {
        // Arrange
        var activity = new GivenGitHubActivity();

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Pre.Weeks[0].Start.ShouldBe(new DateOnly(2024, 12, 29));
    }

    [Fact]
    public void A_week_s_lines_are_summed_across_repos()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo().WithAuthorWeek(PostWeek, added: 100, commits: 1))
            .WithRepo(new GivenAGitHubRepo().WithAuthorWeek(PostWeek, added: 300, commits: 5));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Post.Weeks.Single(w => w.Start == PostWeek).Added.ShouldBe(400);
    }

    [Fact]
    public void Activity_outside_both_windows_is_left_out()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo()
                .WithAuthorWeek(BeforeBothWindows, added: 9_999, commits: 99)
                .WithAuthorWeek(PreWeek, added: 300, commits: 3));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Pre.Weeks.Sum(w => w.Commits).ShouldBe(3);
    }

    [Fact]
    public void A_repo_s_share_is_its_fraction_of_the_window_s_lines()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo()
                .WithAuthorWeek(PreWeek, added: 300, commits: 3)
                .WithAuthorWeek(PostWeek, added: 100, commits: 1))
            .WithRepo(new GivenAGitHubRepo().WithAuthorWeek(PostWeek, added: 300, commits: 5));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos[0].Share.ShouldBe(new PerEra<double>(Pre: 1, Post: 0.25));
    }

    [Fact]
    public void A_repo_created_in_a_window_was_started_in_it()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo().CreatedOn(new DateOnly(2026, 2, 1)));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().StartedIn.ShouldBe(Era.Post);
    }

    [Fact]
    public void A_repo_created_before_both_windows_was_not_started_in_either()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo().CreatedOn(new DateOnly(2020, 1, 1)));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().StartedIn.ShouldBeNull();
    }

    [Fact]
    public void Languages_are_shares_of_the_repo_s_bytes()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo().Writing("C#", 3_000).Writing("SQL", 1_000));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().Languages["C#"].ShouldBe(0.75);
    }

    [Fact]
    public void The_report_names_none_of_the_user_s_projects()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(new GivenAGitHubRepo().WithAuthorWeek(PostWeek, added: 300, commits: 5));

        // Act
        var report = Report.From(activity.Assemble());

        // Assert
        report.Post.Projects.Single().Name.ShouldBeNull();
    }

    [Fact]
    public void A_private_repo_stays_private()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(new GivenAGitHubRepo().Private());

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().IsPrivate.ShouldBeTrue();
    }

    [Fact]
    public void Commits_in_either_window_count_as_activity()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(new GivenAGitHubRepo().WithAuthorWeek(PreWeek, added: 300, commits: 3));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.HasCommits.ShouldBeTrue();
    }

    [Fact]
    public void Commits_only_outside_both_windows_are_no_activity()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(new GivenAGitHubRepo().WithAuthorWeek(BeforeBothWindows, added: 300, commits: 3));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.HasCommits.ShouldBeFalse();
    }

    [Fact]
    public void Active_days_are_counted_within_the_window()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithContributionsOn(new DateOnly(2025, 6, 2), new DateOnly(2026, 6, 8), new DateOnly(2026, 6, 9), BeforeBothWindows);

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Post.ActiveDays.ShouldBe(2);
    }
}
