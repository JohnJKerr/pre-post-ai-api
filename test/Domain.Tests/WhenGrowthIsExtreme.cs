using Shouldly;

namespace Domain.Tests;

public class WhenGrowthIsExtreme
{
    [Fact]
    public void Each_part_caps_at_its_maximum()
    {
        // Arrange
        var shift = ExtremeGrowth();

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.ShouldBe(new ScoreParts(Output: 40, Projects: 15, Languages: 20, Time: 25));
    }

    [Fact]
    public void The_total_is_100()
    {
        // Arrange
        var shift = ExtremeGrowth();

        // Act
        var score = shift.Score();

        // Assert
        score.Total.ShouldBe(100);
    }

    [Fact]
    public void The_verdict_is_I_run_a_lights_out_factory()
    {
        // Arrange
        var shift = ExtremeGrowth();

        // Act
        var score = shift.Score();

        // Assert
        score.Verdict.ShouldBe("I run a lights-out factory");
    }

    private static Shift ExtremeGrowth() => new(
        new GivenAShiftSide().WithLinesAdded(1).WithCommits(1).WithProjects(1).WithLanguages().WithActiveDays(1).Build(),
        new GivenAShiftSide()
            .WithLinesAdded(1_000_000)
            .WithCommits(1_000_000)
            .WithProjects(100)
            .WithLanguages("A", "B", "C", "D", "E", "F")
            .WithActiveDays(100)
            .Build());
}
