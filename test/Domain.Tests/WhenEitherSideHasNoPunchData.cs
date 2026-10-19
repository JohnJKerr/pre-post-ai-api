using Shouldly;

namespace Domain.Tests;

public class WhenEitherSideHasNoPunchData
{
    [Fact]
    public void The_hours_shift_is_undefined()
    {
        // Arrange
        var pre = new GivenAShiftSide().WithOutOfHours(0.1).Build();
        var post = new GivenAShiftSide().WithNoPunchData().Build();

        // Act
        var shift = new Shift(pre, post);

        // Assert
        shift.HoursShift.ShouldBeNull();
    }
}
