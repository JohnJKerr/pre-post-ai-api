using Shouldly;

namespace Domain.Tests;

public class WhenALanguageCrossesTheUsedThreshold
{
    private readonly Report _report = Report.From(new GivenAProfile()
        .WithRepo(new GivenARepo("main").Writing("TypeScript", 0.99).Writing("Go", 0.01).WithShareOfLines(pre: 1, post: 0.5))
        .WithRepo(new GivenARepo("svc").Writing("Go", 1).WithShareOfLines(pre: 0, post: 0.5))
        .Build());

    [Fact]
    public void A_language_under_2_percent_is_not_used() => _report.Pre.UsedLanguages.ShouldBe(["TypeScript"]);

    [Fact]
    public void A_language_over_2_percent_is_used() => _report.Post.UsedLanguages.ShouldContain("Go");

    [Fact]
    public void Shares_are_weighted_by_each_repo_s_share_of_lines() =>
        _report.Post.Languages.Single(l => l.Language == "Go").Share.ShouldBe(0.505, tolerance: 1e-9);

    [Fact]
    public void It_counts_as_new() => _report.Shift.NewLanguages.ShouldBe(["Go"]);

    [Fact]
    public void It_scores_2_5_points() => _report.Score.Parts.Languages.ShouldBe(2.5);
}
