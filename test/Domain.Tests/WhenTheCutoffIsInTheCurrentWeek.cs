using Shouldly;

namespace Domain.Tests;

public class WhenTheCutoffIsInTheCurrentWeek
{
    [Fact]
    public void There_are_no_complete_weeks()
    {
        // Arrange
        var cutoff = new DateOnly(2026, 10, 19);
        var today = new DateOnly(2026, 10, 20);

        // Act
        var window = Window.After(cutoff, today);

        // Assert
        window.Weeks.ShouldBe(0);
    }
}
