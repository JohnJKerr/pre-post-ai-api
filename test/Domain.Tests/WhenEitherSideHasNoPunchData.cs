using Shouldly;

namespace Domain.Tests;

public class WhenEitherSideHasNoPunchData
{
    private readonly Shift _shift = new(
        new GivenAShiftSide().WithOutOfHours(0.1).Build(),
        new GivenAShiftSide().WithNoPunchData().Build());

    [Fact]
    public void The_hours_shift_is_undefined() => _shift.HoursShift.ShouldBeNull();

    [Fact]
    public void Hours_score_0() => _shift.Score().Parts.Hours.ShouldBe(0);

    [Fact]
    public void There_is_no_tag() => _shift.Score().Tag.ShouldBeNull();
}
