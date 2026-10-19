using Shouldly;

namespace Domain.Tests;

public class WhenSettingWindowsForTheDefaultCutoff
{
    private static readonly DateOnly DefaultCutoff = new(2025, 11, 24);
    private static readonly DateOnly Today = new(2026, 10, 18);

    [Fact]
    public void Post_starts_26_weeks_before_the_current_week()
    {
        // Act
        var windows = Window.Around(DefaultCutoff, Today);

        // Assert
        windows.Post.Start.ShouldBe(new DateOnly(2026, 4, 19));
    }

    [Fact]
    public void Post_ends_on_the_last_complete_Saturday()
    {
        // Act
        var windows = Window.Around(DefaultCutoff, Today);

        // Assert
        windows.Post.End.ShouldBe(new DateOnly(2026, 10, 17));
    }

    [Fact]
    public void Post_has_26_weeks()
    {
        // Act
        var windows = Window.Around(DefaultCutoff, Today);

        // Assert
        windows.Post.Weeks.ShouldBe(26);
    }

    [Fact]
    public void Pre_starts_26_weeks_before_the_cutoff_week()
    {
        // Act
        var windows = Window.Around(DefaultCutoff, Today);

        // Assert
        windows.Pre.Start.ShouldBe(new DateOnly(2025, 5, 25));
    }

    [Fact]
    public void Pre_ends_the_day_before_the_cutoff_week()
    {
        // Act
        var windows = Window.Around(DefaultCutoff, Today);

        // Assert
        windows.Pre.End.ShouldBe(new DateOnly(2025, 11, 22));
    }

    [Fact]
    public void Pre_has_26_weeks()
    {
        // Act
        var windows = Window.Around(DefaultCutoff, Today);

        // Assert
        windows.Pre.Weeks.ShouldBe(26);
    }
}
