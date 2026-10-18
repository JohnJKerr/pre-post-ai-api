using Shouldly;

namespace Domain.Tests;

public class WhenOutOfHoursRisesWithoutReaching40Percent
{
    private readonly Shift _shift = new(
        new GivenAShiftSide().WithOutOfHours(0.1).Build(),
        new GivenAShiftSide().WithOutOfHours(0.35).Build());

    [Fact]
    public void The_tag_is_and_I_still_clock_off_on_time() => _shift.Score().Tag.ShouldBe("and I still clock off on time");
}
