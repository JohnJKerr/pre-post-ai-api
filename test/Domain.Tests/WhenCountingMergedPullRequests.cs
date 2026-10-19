using Shouldly;
using static Domain.Tests.GivenGitHubActivity;

namespace Domain.Tests;

public class WhenCountingMergedPullRequests
{
    [Fact]
    public void Pull_requests_merged_after_the_cutoff_count_towards_the_post_window()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithPullRequestsMergedOn(new DateOnly(2025, 6, 2), new DateOnly(2026, 6, 8), new DateOnly(2026, 6, 9));

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Post.MergedPullRequests.ShouldBe(2);
    }

    [Fact]
    public void Pull_requests_merged_before_the_cutoff_count_towards_the_pre_window()
    {
        // Arrange
        var activity = new GivenGitHubActivity()
            .WithPullRequestsMergedOn(new DateOnly(2025, 6, 2), new DateOnly(2026, 6, 8), BeforeBothWindows);

        // Act
        var profile = activity.Assemble();

        // Assert
        profile.Activity.Pre.MergedPullRequests.ShouldBe(1);
    }
}
