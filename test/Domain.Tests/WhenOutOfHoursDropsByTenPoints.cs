using Shouldly;

namespace Domain.Tests;

public class WhenOutOfHoursDropsByTenPoints
{
    private readonly Shift _shift = new(
        new GivenAShiftSide().WithOutOfHours(0.3).Build(),
        new GivenAShiftSide().WithOutOfHours(0.2).Build());

    [Fact]
    public void The_hours_shift_is_exactly_minus_10_points() => _shift.HoursShift.ShouldBe(-10);

    [Fact]
    public void Hours_score_6() => _shift.Score().Parts.Hours.ShouldBe(6);

    [Fact]
    public void The_tag_is_and_I_got_my_evenings_back() => _shift.Score().Tag.ShouldBe("and I got my evenings back");
}
