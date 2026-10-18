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

    [Fact]
    public void Hours_score_0()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithOutOfHours(0.1).Build(), new GivenAShiftSide().WithNoPunchData().Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Hours.ShouldBe(0);
    }

    [Fact]
    public void There_is_no_tag()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithOutOfHours(0.1).Build(), new GivenAShiftSide().WithNoPunchData().Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Tag.ShouldBeNull();
    }
}
