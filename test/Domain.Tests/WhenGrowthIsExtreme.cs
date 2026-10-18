using Shouldly;

namespace Domain.Tests;

public class WhenGrowthIsExtreme
{
    private readonly Score _score;

    public WhenGrowthIsExtreme()
    {
        var pre = new GivenAShiftSide().WithLinesAdded(1).WithProjects(1).WithLanguages().WithOutOfHours(0).Build();
        var post = new GivenAShiftSide()
            .WithLinesAdded(1_000_000)
            .WithProjects(100)
            .WithLanguages("A", "B", "C", "D", "E", "F")
            .WithOutOfHours(1)
            .Build();
        _score = new Shift(pre, post).Score();
    }

    [Fact]
    public void Each_part_caps_at_its_maximum() =>
        _score.Parts.ShouldBe(new ScoreParts(Output: 45, Projects: 15, Languages: 10, Hours: 30));

    [Fact]
    public void The_total_is_100() => _score.Total.ShouldBe(100);

    [Fact]
    public void The_verdict_is_I_run_a_lights_out_factory() => _score.Verdict.ShouldBe("I run a lights-out factory");
}
