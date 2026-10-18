using Shouldly;

namespace Domain.Tests;

public class WhenTheCutoffIsInTheCurrentWeek
{
    [Fact]
    public void There_are_no_complete_weeks() =>
        Window.After(new DateOnly(2026, 10, 19), today: new DateOnly(2026, 10, 20)).Weeks.ShouldBe(0);
}
