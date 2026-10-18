using Shouldly;

namespace Domain.Tests;

public class WhenANewWeekStarts
{
    [Fact]
    public void The_week_just_finished_is_included() =>
        Window.After(new DateOnly(2025, 11, 24), today: new DateOnly(2026, 10, 25)).Weeks.ShouldBe(48);
}
