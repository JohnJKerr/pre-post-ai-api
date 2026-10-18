using Shouldly;

namespace Domain.Tests;

public class WhenThereWasNoPreAiActivity
{
    private readonly Shift _shift = new(
        new GivenAShiftSide().WithNoActivity().Build(),
        new GivenAShiftSide().Build());

    [Fact]
    public void The_lines_multiplier_is_undefined() => _shift.LinesX.ShouldBeNull();

    [Fact]
    public void The_projects_multiplier_is_undefined() => _shift.ProjectsX.ShouldBeNull();

    [Fact]
    public void Output_scores_0() => _shift.Score().Parts.Output.ShouldBe(0);

    [Fact]
    public void Projects_score_0() => _shift.Score().Parts.Projects.ShouldBe(0);
}
