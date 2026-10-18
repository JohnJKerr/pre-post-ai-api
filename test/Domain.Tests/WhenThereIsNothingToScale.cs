using Shouldly;

namespace Domain.Tests;

public class WhenThereIsNothingToScale
{
    [Fact]
    public void Weeks_scale_to_at_least_1()
    {
        // Arrange
        var profile = new GivenAProfile().WithLinesAdded(before: 0, after: 0).Build();

        // Act
        var scales = Report.From(profile).Scales;

        // Assert
        scales.WeekMax.ShouldBe(1);
    }

    [Fact]
    public void Projects_scale_to_at_least_1()
    {
        // Arrange
        var profile = new GivenAProfile().WithLinesAdded(before: 0, after: 0).Build();

        // Act
        var scales = Report.From(profile).Scales;

        // Assert
        scales.ProjectMax.ShouldBe(1);
    }

    [Fact]
    public void Punch_cards_scale_to_at_least_1()
    {
        // Arrange
        var profile = new GivenAProfile().WithLinesAdded(before: 0, after: 0).Build();

        // Act
        var scales = Report.From(profile).Scales;

        // Assert
        scales.CellMax.ShouldBe(1);
    }

    [Fact]
    public void Hours_scale_to_at_least_1_percent_with_headroom()
    {
        // Arrange
        var profile = new GivenAProfile().WithLinesAdded(before: 0, after: 0).Build();

        // Act
        var scales = Report.From(profile).Scales;

        // Assert
        scales.HourMax.ShouldBe(0.0108, tolerance: 1e-9);
    }
}
