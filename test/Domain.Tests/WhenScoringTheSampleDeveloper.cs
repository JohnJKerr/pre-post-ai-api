using Shouldly;

namespace Domain.Tests;

public class WhenScoringTheSampleDeveloper
{
    private readonly Score _score;

    public WhenScoringTheSampleDeveloper()
    {
        var pre = new GivenAShiftSide()
            .WithLinesAdded(40_459)
            .WithProjects(5)
            .WithLanguages("C#", "TypeScript", "Shell", "JavaScript", "SQL")
            .WithOutOfHours(0.122)
            .Build();
        var post = new GivenAShiftSide()
            .WithLinesAdded(226_832)
            .WithProjects(12)
            .WithLanguages("TypeScript", "C#", "Python", "Go", "Rust", "Swift", "SQL", "Shell")
            .WithOutOfHours(0.546)
            .Build();
        _score = new Shift(pre, post).Score();
    }

    [Fact]
    public void Output_scores_34() => _score.Parts.Output.ShouldBe(34);

    [Fact]
    public void Projects_score_9() => _score.Parts.Projects.ShouldBe(9);

    [Fact]
    public void Languages_score_10() => _score.Parts.Languages.ShouldBe(10);

    [Fact]
    public void Hours_score_25() => _score.Parts.Hours.ShouldBe(25);

    [Fact]
    public void The_total_is_78() => _score.Total.ShouldBe(78);

    [Fact]
    public void The_verdict_is_I_built_a_software_factory() => _score.Verdict.ShouldBe("I built a software factory");

    [Fact]
    public void The_tag_is_and_it_runs_nights_and_weekends() => _score.Tag.ShouldBe("and it runs nights and weekends");
}
