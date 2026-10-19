using Shouldly;

namespace Domain.Tests;

public class WhenTheCutoffIsLessThan26WeeksAgo
{
    private static readonly DateOnly Cutoff = new(2026, 7, 1);
    private static readonly DateOnly Today = new(2026, 10, 18);

    [Fact]
    public void Post_starts_on_the_Sunday_of_the_cutoff_week()
    {
        // Act
        var windows = Window.Around(Cutoff, Today);

        // Assert
        windows.Post.Start.ShouldBe(new DateOnly(2026, 6, 28));
    }

    [Fact]
    public void Post_has_every_complete_week_since_the_cutoff()
    {
        // Act
        var windows = Window.Around(Cutoff, Today);

        // Assert
        windows.Post.Weeks.ShouldBe(16);
    }

    [Fact]
    public void Pre_has_as_many_weeks_as_post()
    {
        // Act
        var windows = Window.Around(Cutoff, Today);

        // Assert
        windows.Pre.Weeks.ShouldBe(16);
    }

    [Fact]
    public void Pre_ends_the_day_before_the_cutoff_week()
    {
        // Act
        var windows = Window.Around(Cutoff, Today);

        // Assert
        windows.Pre.End.ShouldBe(new DateOnly(2026, 6, 27));
    }
}
