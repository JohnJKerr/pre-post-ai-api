using Shouldly;

namespace Domain.Tests;

public class WhenThereWasNoPreAiActivity
{
    [Fact]
    public void The_lines_multiplier_is_undefined()
    {
        // Arrange
        var pre = new GivenAShiftSide().WithNoActivity().Build();
        var post = new GivenAShiftSide().Build();

        // Act
        var shift = new Shift(pre, post);

        // Assert
        shift.LinesX.ShouldBeNull();
    }

    [Fact]
    public void The_projects_multiplier_is_undefined()
    {
        // Arrange
        var pre = new GivenAShiftSide().WithNoActivity().Build();
        var post = new GivenAShiftSide().Build();

        // Act
        var shift = new Shift(pre, post);

        // Assert
        shift.ProjectsX.ShouldBeNull();
    }

    [Fact]
    public void Output_scores_0()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithNoActivity().Build(), new GivenAShiftSide().Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Output.ShouldBe(0);
    }

    [Fact]
    public void Projects_score_0()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithNoActivity().Build(), new GivenAShiftSide().Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Projects.ShouldBe(0);
    }

    [Fact]
    public void Time_scores_0()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithNoActivity().Build(), new GivenAShiftSide().Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Time.ShouldBe(0);
    }
}
