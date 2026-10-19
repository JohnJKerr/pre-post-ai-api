using Shouldly;

namespace Domain.Tests;

public class WhenCommitsAndLinesGrowUnevenly
{
    [Fact]
    public void Ten_times_the_commits_alone_scores_28()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithCommits(100).Build(), new GivenAShiftSide().WithCommits(1_000).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Output.ShouldBe(28);
    }

    [Fact]
    public void Ten_times_the_lines_alone_scores_12()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithLinesAdded(10_000).Build(), new GivenAShiftSide().WithLinesAdded(100_000).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Output.ShouldBe(12);
    }
}
