using Shouldly;

namespace Domain.Tests;

public class WhenSettingWindowsForTheDefaultCutoff
{
    private static readonly DateOnly DefaultCutoff = new(2025, 11, 24);
    private static readonly DateOnly Today = new(2026, 10, 18);

    [Fact]
    public void Post_starts_on_the_Sunday_of_the_cutoff_week()
    {
        // Act
        var post = Window.After(DefaultCutoff, Today);

        // Assert
        post.Start.ShouldBe(new DateOnly(2025, 11, 23));
    }

    [Fact]
    public void Post_ends_on_the_last_complete_Saturday()
    {
        // Act
        var post = Window.After(DefaultCutoff, Today);

        // Assert
        post.End.ShouldBe(new DateOnly(2026, 10, 17));
    }

    [Fact]
    public void Post_has_47_complete_weeks()
    {
        // Act
        var post = Window.After(DefaultCutoff, Today);

        // Assert
        post.Weeks.ShouldBe(47);
    }

    [Fact]
    public void Pre_starts_47_weeks_before_the_cutoff_week()
    {
        // Arrange
        var post = Window.After(DefaultCutoff, Today);

        // Act
        var pre = post.Preceding();

        // Assert
        pre.Start.ShouldBe(new DateOnly(2024, 12, 29));
    }

    [Fact]
    public void Pre_ends_the_day_before_the_cutoff_week()
    {
        // Arrange
        var post = Window.After(DefaultCutoff, Today);

        // Act
        var pre = post.Preceding();

        // Assert
        pre.End.ShouldBe(new DateOnly(2025, 11, 22));
    }

    [Fact]
    public void Pre_has_as_many_weeks_as_post()
    {
        // Arrange
        var post = Window.After(DefaultCutoff, Today);

        // Act
        var pre = post.Preceding();

        // Assert
        pre.Weeks.ShouldBe(47);
    }
}
