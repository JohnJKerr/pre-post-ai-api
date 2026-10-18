using Shouldly;

namespace Domain.Tests;

public class WhenScalingTheCharts
{
    private readonly Scales _scales = Report.From(new GivenAProfile()
        .WithLinesAdded(before: 1_000, after: 2_500)
        .WithRepo(new GivenARepo("app").WithShareOfLines(pre: 0.2, post: 0.6).WithPunchCardFor(Era.Post))
        .WithPunchCardAfterTheCutoff(new PunchCell(Day: 1, Hour: 9, Commits: 3), new PunchCell(Day: 1, Hour: 10, Commits: 1))
        .Build()).Scales;

    [Fact]
    public void Weeks_share_the_largest_week_across_both_windows() => _scales.WeekMax.ShouldBe(2_500);

    [Fact]
    public void Projects_share_the_largest_project_across_both_windows() => _scales.ProjectMax.ShouldBe(1_500);

    [Fact]
    public void Punch_cards_share_the_largest_cell_across_both_windows() => _scales.CellMax.ShouldBe(3);

    [Fact]
    public void Hours_share_the_tallest_hour_with_8_percent_headroom() => _scales.HourMax.ShouldBe(0.81, tolerance: 1e-9);
}
