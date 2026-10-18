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

    [Fact]
    public void Hours_score_6()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithOutOfHours(0.3).Build(), new GivenAShiftSide().WithOutOfHours(0.2).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Hours.ShouldBe(6);
    }

    [Fact]
    public void The_tag_is_and_I_got_my_evenings_back()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithOutOfHours(0.3).Build(), new GivenAShiftSide().WithOutOfHours(0.2).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Tag.ShouldBe("and I got my evenings back");
    }
}
