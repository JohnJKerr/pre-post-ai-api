using Shouldly;

namespace Domain.Tests;

public class WhenANewWeekStarts
{
    [Fact]
    public void The_week_just_finished_is_included()
    {
        // Arrange
        var cutoff = new DateOnly(2025, 11, 24);
        var today = new DateOnly(2026, 10, 25);

        // Act
        var window = Window.After(cutoff, today);

        // Assert
        window.Weeks.ShouldBe(48);
    }
}
