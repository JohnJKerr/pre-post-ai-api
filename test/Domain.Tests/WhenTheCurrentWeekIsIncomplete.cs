using Shouldly;

namespace Domain.Tests;

public class WhenTheCurrentWeekIsIncomplete
{
    [Fact]
    public void The_current_week_is_left_out() =>
        Window.After(new DateOnly(2025, 11, 24), today: new DateOnly(2026, 10, 21)).Weeks.ShouldBe(47);
}
