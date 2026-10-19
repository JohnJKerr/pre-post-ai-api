using Shouldly;

namespace Domain.Tests;

public class WhenScoringTheSampleDeveloper
{
    [Fact]
    public void Output_scores_20()
    {
        // Arrange
        var shift = new Shift(
            new GivenAShiftSide().WithLinesAdded(40_459).WithCommits(638).Build(),
            new GivenAShiftSide().WithLinesAdded(226_832).WithCommits(1_604).Build());

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
    public void Time_scores_20()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithActiveDays(161).Build(), new GivenAShiftSide().WithActiveDays(277).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Time.ShouldBe(20);
    }

    [Fact]
    public void The_total_is_69()
    {
        // Arrange
        var shift = TheSampleDevelopersShift();

        // Act
        var score = shift.Score();

        // Assert
        score.Total.ShouldBe(69);
    }

    [Fact]
    public void The_verdict_is_I_built_a_software_factory()
    {
        // Arrange
        var shift = TheSampleDevelopersShift();

        // Act
        var score = shift.Score();

        // Assert
        score.Verdict.ShouldBe("I built a software factory");
    }

    [Fact]
    public void The_tag_is_and_it_runs_nights_and_weekends()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithOutOfHours(0.122).Build(), new GivenAShiftSide().WithOutOfHours(0.546).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Tag.ShouldBe("and it runs nights and weekends");
    }

    private static Shift TheSampleDevelopersShift() => new(
        new GivenAShiftSide()
            .WithLinesAdded(40_459)
            .WithCommits(638)
            .WithProjects(5)
            .WithLanguages("C#", "TypeScript", "Shell", "JavaScript", "SQL")
            .WithActiveDays(161)
            .WithOutOfHours(0.122)
            .Build(),
        new GivenAShiftSide()
            .WithLinesAdded(226_832)
            .WithCommits(1_604)
            .WithProjects(12)
            .WithLanguages("TypeScript", "C#", "Python", "Go", "Rust", "Swift", "SQL", "Shell")
            .WithActiveDays(277)
            .WithOutOfHours(0.546)
            .Build());
}
