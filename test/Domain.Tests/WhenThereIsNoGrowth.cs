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
    public void The_verdict_is_I_m_an_artisan()
    {
        // Arrange
        var side = new GivenAShiftSide().Build();

        // Act
        var score = new Shift(side, side).Score();

        // Assert
        score.Verdict.ShouldBe("I'm an artisan");
    }
}
