using Shouldly;

namespace Domain.Tests;

public class WhenScalingTheCharts
{
    [Fact]
    public void Weeks_share_the_largest_week_across_both_windows()
    {
        // Arrange
        var profile = new GivenAProfile()
            .WithLinesAdded(before: 1_000, after: 2_500)
            .WithRepo(new GivenARepo("app").WithShareOfLines(pre: 0.2, post: 0.6).WithPunchCardFor(Era.Post))
            .WithPunchCardAfterTheCutoff(new PunchCell(Day: 1, Hour: 9, Commits: 3), new PunchCell(Day: 1, Hour: 10, Commits: 1))
            .Build();

        // Act
        var scales = Report.From(profile).Scales;

        // Assert
        scales.WeekMax.ShouldBe(2_500);
    }

    [Fact]
    public void Projects_share_the_largest_project_across_both_windows()
    {
        // Arrange
        var profile = new GivenAProfile()
            .WithLinesAdded(before: 1_000, after: 2_500)
            .WithRepo(new GivenARepo("app").WithShareOfLines(pre: 0.2, post: 0.6).WithPunchCardFor(Era.Post))
            .WithPunchCardAfterTheCutoff(new PunchCell(Day: 1, Hour: 9, Commits: 3), new PunchCell(Day: 1, Hour: 10, Commits: 1))
            .Build();

        // Act
        var scales = Report.From(profile).Scales;

        // Assert
        scales.ProjectMax.ShouldBe(1_500);
    }

    [Fact]
    public void Punch_cards_share_the_largest_cell_across_both_windows()
    {
        // Arrange
        var profile = new GivenAProfile()
            .WithLinesAdded(before: 1_000, after: 2_500)
            .WithRepo(new GivenARepo("app").WithShareOfLines(pre: 0.2, post: 0.6).WithPunchCardFor(Era.Post))
            .WithPunchCardAfterTheCutoff(new PunchCell(Day: 1, Hour: 9, Commits: 3), new PunchCell(Day: 1, Hour: 10, Commits: 1))
            .Build();

        // Act
        var scales = Report.From(profile).Scales;

        // Assert
        scales.CellMax.ShouldBe(3);
    }

    [Fact]
    public void Hours_share_the_tallest_hour_with_8_percent_headroom()
    {
        // Arrange
        var profile = new GivenAProfile()
            .WithLinesAdded(before: 1_000, after: 2_500)
            .WithRepo(new GivenARepo("app").WithShareOfLines(pre: 0.2, post: 0.6).WithPunchCardFor(Era.Post))
            .WithPunchCardAfterTheCutoff(new PunchCell(Day: 1, Hour: 9, Commits: 3), new PunchCell(Day: 1, Hour: 10, Commits: 1))
            .Build();

        // Act
        var scales = Report.From(profile).Scales;

        // Assert
        scales.HourMax.ShouldBe(0.81, tolerance: 1e-9);
    }
}
