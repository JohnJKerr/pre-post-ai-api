using Shouldly;

namespace Domain.Tests;

public class WhenThereIsNoGrowth
{
    [Fact]
    public void The_total_is_0()
    {
        // Arrange
        var side = new GivenAShiftSide().Build();

        // Act
        var score = new Shift(side, side).Score();

        // Assert
        score.Total.ShouldBe(0);
    }

    [Fact]
    public void The_verdict_is_I_m_still_hand_crafting()
    {
        // Arrange
        var side = new GivenAShiftSide().Build();

        // Act
        var score = new Shift(side, side).Score();

        // Assert
        score.Verdict.ShouldBe("I'm still hand-crafting");
    }

    [Fact]
    public void The_tag_is_and_I_still_clock_off_on_time()
    {
        // Arrange
        var side = new GivenAShiftSide().Build();

        // Act
        var score = new Shift(side, side).Score();

        // Assert
        score.Tag.ShouldBe("and I still clock off on time");
    }
}
