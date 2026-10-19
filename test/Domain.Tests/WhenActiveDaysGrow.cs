using Shouldly;

namespace Domain.Tests;

public class WhenActiveDaysGrow
{
    [Fact]
    public void Doubling_them_scores_the_full_25()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithActiveDays(50).Build(), new GivenAShiftSide().WithActiveDays(100).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Time.ShouldBe(25);
    }

    [Fact]
    public void Half_as_many_again_scores_15()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithActiveDays(100).Build(), new GivenAShiftSide().WithActiveDays(150).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Time.ShouldBe(15);
    }

    [Fact]
    public void Fewer_score_0()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithActiveDays(100).Build(), new GivenAShiftSide().WithActiveDays(60).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Time.ShouldBe(0);
    }

    [Fact]
    public void The_multiplier_is_post_over_pre()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithActiveDays(100).Build(), new GivenAShiftSide().WithActiveDays(150).Build());

        // Act
        var multiplier = shift.ActiveDaysX;

        // Assert
        multiplier.ShouldBe(1.5);
    }
}
