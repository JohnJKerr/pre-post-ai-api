using Shouldly;

namespace Domain.Tests;

public class WhenThereIsNothingToScale
{
    private readonly Scales _scales = Report.From(new GivenAProfile().WithLinesAdded(before: 0, after: 0).Build()).Scales;

    [Fact]
    public void Weeks_scale_to_at_least_1() => _scales.WeekMax.ShouldBe(1);

    [Fact]
    public void Projects_scale_to_at_least_1() => _scales.ProjectMax.ShouldBe(1);

    [Fact]
    public void Punch_cards_scale_to_at_least_1() => _scales.CellMax.ShouldBe(1);

    [Fact]
    public void Hours_scale_to_at_least_1_percent_with_headroom() => _scales.HourMax.ShouldBe(0.0108, tolerance: 1e-9);
}
