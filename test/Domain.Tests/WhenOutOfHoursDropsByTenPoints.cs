using Shouldly;

namespace Domain.Tests;

public class WhenOutOfHoursDropsByTenPoints
{
    [Fact]
    public void The_hours_shift_is_exactly_minus_10_points()
    {
        // Arrange
        var pre = new GivenAShiftSide().WithOutOfHours(0.3).Build();
        var post = new GivenAShiftSide().WithOutOfHours(0.2).Build();

        // Act
        var shift = new Shift(pre, post);

        // Assert
        shift.HoursShift.ShouldBe(-10);
    }
}
