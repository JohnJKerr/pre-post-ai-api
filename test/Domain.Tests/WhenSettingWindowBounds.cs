using Shouldly;

namespace Domain.Tests;

public class WhenSettingWindowBounds
{
    private static readonly DateOnly DefaultCutoff = new(2025, 11, 24);

    [Fact]
    public void Default_cut_off_gives_47_weeks_either_side()
    {
        var post = Window.After(DefaultCutoff, new DateOnly(2026, 10, 18));
        var pre = post.Preceding();

        post.Weeks.ShouldBe(47);
        pre.Weeks.ShouldBe(47);
        (post.Start, post.End).ShouldBe((new DateOnly(2025, 11, 23), new DateOnly(2026, 10, 17)));
        (pre.Start, pre.End).ShouldBe((new DateOnly(2024, 12, 29), new DateOnly(2025, 11, 22)));
    }

    [Fact]
    public void A_partial_current_week_is_left_out()
    {
        Window.After(DefaultCutoff, new DateOnly(2026, 10, 21)).Weeks.ShouldBe(47);
        Window.After(DefaultCutoff, new DateOnly(2026, 10, 25)).Weeks.ShouldBe(48);
    }

    [Fact]
    public void A_cut_off_in_the_current_week_has_no_complete_weeks()
    {
        Window.After(new DateOnly(2026, 10, 19), new DateOnly(2026, 10, 20)).Weeks.ShouldBe(0);
    }
}
