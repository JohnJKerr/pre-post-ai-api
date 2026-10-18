using Shouldly;

namespace Domain.Tests;

public class WhenTheCurrentWeekIsIncomplete
{
    [Fact]
    public void The_current_week_is_left_out()
    {
        // Arrange
        var cutoff = new DateOnly(2025, 11, 24);
        var today = new DateOnly(2026, 10, 21);

        // Act
        var window = Window.After(cutoff, today);

        // Assert
        window.Weeks.ShouldBe(47);
    }
}
