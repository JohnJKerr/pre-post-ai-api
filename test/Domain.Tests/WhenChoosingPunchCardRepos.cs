using Shouldly;
using static Domain.Tests.GivenGitHubActivity;

namespace Domain.Tests;

public class WhenChoosingPunchCardRepos
{
    [Fact]
    public void A_repo_the_author_wrote_90_percent_of_after_the_cutoff_is_eligible_after_it()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(ASoloRepo());

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().PunchCardEra.ShouldBe(Era.Post);
    }

    [Fact]
    public void A_repo_others_wrote_more_than_a_tenth_of_is_not_eligible()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(ASharedRepo());

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().PunchCardEra.ShouldBeNull();
    }

    [Fact]
    public void A_repo_with_activity_either_side_of_the_cutoff_is_not_eligible()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo()
                .WithAuthorWeek(PreWeek, added: 300, commits: 3)
                .WithAuthorWeek(PostWeek, added: 100, commits: 1)
                .WithPunchCell(day: 0, hour: 0, commits: 4));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().PunchCardEra.ShouldBeNull();
    }

    [Fact]
    public void A_repo_with_activity_between_the_cutoff_and_the_post_window_is_eligible_after_it()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo()
                .WithAuthorWeek(BetweenTheWindows, added: 300, commits: 3)
                .WithAuthorWeek(PostWeek, added: 100, commits: 1)
                .WithPunchCell(day: 0, hour: 0, commits: 4));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().PunchCardEra.ShouldBe(Era.Post);
    }

    [Fact]
    public void A_repo_with_no_commits_in_either_window_is_not_eligible()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithRepo(new GivenAGitHubRepo()
                .WithAuthorWeek(BeforeBothWindows, added: 500, commits: 5)
                .WithPunchCell(day: 4, hour: 4, commits: 5));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Repos.Single().PunchCardEra.ShouldBeNull();
    }

    [Fact]
    public void The_window_s_punch_card_sums_only_eligible_repos()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(ASoloRepo()).WithRepo(ASharedRepo());

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Post.PunchCard.Sum(c => c.Commits).ShouldBe(10);
    }

    [Fact]
    public void The_punch_card_has_every_hour_of_the_week()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(ASoloRepo());

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Post.PunchCard.Count.ShouldBe(168);
    }

    [Fact]
    public void Punch_share_is_the_eligible_repos_share_of_the_window_s_commits()
    {
        // Arrange
        var activity = new GivenGitHubActivity().WithRepo(ASoloRepo()).WithRepo(ASharedRepo());

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Post.PunchShare.ShouldBe(9 / 17d);
    }

    private static GivenAGitHubRepo ASoloRepo() => new GivenAGitHubRepo()
        .WithAuthorWeek(PostWeek, added: 900, commits: 9)
        .WithCommitsByOthers(1)
        .WithPunchCell(day: 2, hour: 21, commits: 6)
        .WithPunchCell(day: 3, hour: 22, commits: 4);

    private static GivenAGitHubRepo ASharedRepo() => new GivenAGitHubRepo()
        .WithAuthorWeek(PostWeek, added: 800, commits: 8)
        .WithCommitsByOthers(2)
        .WithPunchCell(day: 1, hour: 10, commits: 10);
}
