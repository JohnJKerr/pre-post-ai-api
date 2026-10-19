using Shouldly;

namespace Domain.Tests;

public class WhenALanguageIsNew
{
    [Fact]
    public void It_needs_at_least_5_percent_after_the_cutoff()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().Build(), new GivenAShiftSide().WithLanguage("Go", 0.049).Build());

        // Act
        var newLanguages = shift.NewLanguages;

        // Assert
        newLanguages.ShouldBeEmpty();
    }

    [Fact]
    public void It_can_have_had_up_to_1_percent_before()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithLanguage("Go", 0.01).Build(), new GivenAShiftSide().WithLanguage("Go", 0.05).Build());

        // Act
        var newLanguages = shift.NewLanguages;

        // Assert
        newLanguages.ShouldBe(["Go"]);
    }

    [Fact]
    public void One_used_more_than_1_percent_before_is_not_new()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().WithLanguage("Go", 0.011).Build(), new GivenAShiftSide().WithLanguage("Go", 0.5).Build());

        // Act
        var newLanguages = shift.NewLanguages;

        // Assert
        newLanguages.ShouldBeEmpty();
    }

    [Fact]
    public void Each_scores_5_points()
    {
        // Arrange
        var shift = new Shift(new GivenAShiftSide().Build(), new GivenAShiftSide().WithLanguage("Go", 0.1).WithLanguage("Rust", 0.1).Build());

        // Act
        var score = shift.Score();

        // Assert
        score.Parts.Languages.ShouldBe(10);
    }
}
