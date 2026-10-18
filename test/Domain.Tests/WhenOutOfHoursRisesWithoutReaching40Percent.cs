using Shouldly;

namespace Domain.Tests;

public class WhenOutOfHoursRisesWithoutReaching40Percent
{
    [Fact]
    public void The_tag_is_and_I_still_clock_off_on_time()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithOutOfHours(0.1).Build(), new GivenAShiftSide().WithOutOfHours(0.35).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Tag.ShouldBe("and I still clock off on time");
    }
}
