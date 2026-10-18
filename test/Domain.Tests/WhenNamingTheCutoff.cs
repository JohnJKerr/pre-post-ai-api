using Shouldly;

namespace Domain.Tests;

public class WhenNamingTheCutoff
{
    [Fact]
    public void The_Claude_Opus_4_5_release_is_named() => Cutoff.On(new DateOnly(2025, 11, 24)).Label.ShouldBe("Claude Opus 4.5");

    [Fact]
    public void The_ChatGPT_launch_is_named() => Cutoff.On(new DateOnly(2022, 11, 30)).Label.ShouldBe("ChatGPT launch");

    [Fact]
    public void Any_other_date_is_custom() => Cutoff.On(new DateOnly(2024, 3, 1)).Label.ShouldBe("Custom");
}
