using Shouldly;

namespace Domain.Tests;

public class WhenThereIsNoGrowth
{
    private readonly Score _score;

    public WhenThereIsNoGrowth()
    {
        var side = new GivenAShiftSide().Build();
        _score = new Shift(side, side).Score();
    }

    [Fact]
    public void The_total_is_0() => _score.Total.ShouldBe(0);

    [Fact]
    public void The_verdict_is_I_m_still_hand_crafting() => _score.Verdict.ShouldBe("I'm still hand-crafting");

    [Fact]
    public void The_tag_is_and_I_still_clock_off_on_time() => _score.Tag.ShouldBe("and I still clock off on time");
}
