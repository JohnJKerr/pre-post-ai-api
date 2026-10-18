using Shouldly;

namespace Domain.Tests;

public class WhenScoringTheSampleDeveloper
{
    [Fact]
    public void Output_scores_34()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithLinesAdded(40_459).Build(), new GivenAShiftSide().WithLinesAdded(226_832).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Output.ShouldBe(34);
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
    public void Languages_score_10()
    {
        // Arrange
        var shift = new Shift(
            new GivenAShiftSide().WithLanguages("C#", "TypeScript", "Shell", "JavaScript", "SQL").Build(),
            new GivenAShiftSide().WithLanguages("TypeScript", "C#", "Python", "Go", "Rust", "Swift", "SQL", "Shell").Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Languages.ShouldBe(10);
    }

    [Fact]
    public void Hours_score_25()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithOutOfHours(0.122).Build(), new GivenAShiftSide().WithOutOfHours(0.546).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Hours.ShouldBe(25);
    }

    [Fact]
    public void The_total_is_78()
    {
        // Arrange
        var shift = TheSampleDevelopersShift();

        // Act
        var score = shift.Score();

        // Assert
        score.Total.ShouldBe(78);
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
            .WithProjects(5)
            .WithLanguages("C#", "TypeScript", "Shell", "JavaScript", "SQL")
            .WithOutOfHours(0.122)
            .Build(),
        new GivenAShiftSide()
            .WithLinesAdded(226_832)
            .WithProjects(12)
            .WithLanguages("TypeScript", "C#", "Python", "Go", "Rust", "Swift", "SQL", "Shell")
            .WithOutOfHours(0.546)
            .Build());
}
