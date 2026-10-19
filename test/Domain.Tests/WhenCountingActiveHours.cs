using Shouldly;
using static Domain.Tests.GivenGitHubActivity;

namespace Domain.Tests;

public class WhenCountingActiveHours
{
    [Fact]
    public void Each_hour_with_a_commit_counts_once()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithCommitsAt(new DateTime(2026, 6, 8, 21, 5, 0), new DateTime(2026, 6, 8, 21, 40, 0), new DateTime(2026, 6, 8, 22, 10, 0));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Post.ActiveHours.ShouldBe(2);
    }

    [Fact]
    public void Hours_before_the_cutoff_count_towards_the_pre_window()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithCommitsAt(new DateTime(2025, 6, 2, 9, 0, 0), new DateTime(2026, 6, 8, 21, 0, 0));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Pre.ActiveHours.ShouldBe(1);
    }

    [Fact]
    public void Hours_outside_both_windows_are_left_out()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithCommitsAt(BeforeBothWindows.ToDateTime(new TimeOnly(9, 0)), new DateTime(2025, 6, 2, 9, 0, 0));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Pre.ActiveHours.ShouldBe(1);
    }
}
