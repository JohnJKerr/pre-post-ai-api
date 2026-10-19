using Shouldly;

namespace Domain.Tests;

public class WhenScoringTheSampleDeveloper
{
    [Fact]
    public void Output_scores_20()
    {
        // Arrange
        var shift = new Shift(
            new GivenAShiftSide().WithLinesAdded(38_306).WithCommits(639).Build(),
            new GivenAShiftSide().WithLinesAdded(232_608).WithCommits(1_617).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Output.ShouldBe(20);
    }

    [Fact]
    public void Projects_score_9()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithProjects(5).Build(), new GivenAShiftSide().WithProjects(12).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Projects.ShouldBe(9);
    }

    [Fact]
    public void Languages_score_20()
    {
        // Arrange
        var shift = new Shift(
            new GivenAShiftSide().WithLanguages("C#", "TypeScript", "Shell", "JavaScript", "SQL").Build(),
            new GivenAShiftSide().WithLanguages("TypeScript", "C#", "Python", "Go", "Rust", "Swift", "SQL", "Shell").Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Languages.ShouldBe(20);
    }

    [Fact]
    public void Time_scores_16()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithActiveDays(99).Build(), new GivenAShiftSide().WithActiveDays(155).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Time.ShouldBe(16);
    }

    [Fact]
    public void The_total_is_65()
    {
        // Arrange
        var shift = TheSampleDevelopersShift();

        // Act
        var score = shift.Score();

        // Assert
        score.Total.ShouldBe(65);
    }

    [Fact]
    public void The_verdict_is_I_m_a_machine_literally()
    {
        // Arrange
        var shift = TheSampleDevelopersShift();

        // Act
        var score = shift.Score();

        // Assert
        score.Verdict.ShouldBe("I'm a machine (literally)");
    }

    private static Shift TheSampleDevelopersShift() => new(
        new GivenAShiftSide()
            .WithLinesAdded(38_306)
            .WithCommits(639)
            .WithProjects(5)
            .WithLanguages("C#", "TypeScript", "Shell", "JavaScript", "SQL")
            .WithActiveDays(99)
            .WithOutOfHours(0.126)
            .Build(),
        new GivenAShiftSide()
            .WithLinesAdded(232_608)
            .WithCommits(1_617)
            .WithProjects(12)
            .WithLanguages("TypeScript", "C#", "Python", "Go", "Rust", "Swift", "SQL", "Shell")
            .WithActiveDays(155)
            .WithOutOfHours(0.552)
            .Build());
}
