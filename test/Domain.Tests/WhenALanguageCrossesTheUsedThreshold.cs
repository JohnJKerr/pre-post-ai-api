using Shouldly;

namespace Domain.Tests;

public class WhenALanguageCrossesTheUsedThreshold
{
    [Fact]
    public void A_language_under_2_percent_is_not_used()
    {
        // Arrange
        var profile = new GivenAProfile()
            .WithRepo(new GivenARepo("main").Writing("TypeScript", 0.99).Writing("Go", 0.01).WithShareOfLines(pre: 1, post: 0.5))
            .WithRepo(new GivenARepo("svc").Writing("Go", 1).WithShareOfLines(pre: 0, post: 0.5))
            .Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.UsedLanguages.ShouldBe(["TypeScript"]);
    }

    [Fact]
    public void A_language_over_2_percent_is_used()
    {
        // Arrange
        var profile = new GivenAProfile()
            .WithRepo(new GivenARepo("main").Writing("TypeScript", 0.99).Writing("Go", 0.01).WithShareOfLines(pre: 1, post: 0.5))
            .WithRepo(new GivenARepo("svc").Writing("Go", 1).WithShareOfLines(pre: 0, post: 0.5))
            .Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.UsedLanguages.ShouldContain("Go");
    }

    [Fact]
    public void Shares_are_weighted_by_each_repo_s_share_of_lines()
    {
        // Arrange
        var profile = new GivenAProfile()
            .WithRepo(new GivenARepo("main").Writing("TypeScript", 0.99).Writing("Go", 0.01).WithShareOfLines(pre: 1, post: 0.5))
            .WithRepo(new GivenARepo("svc").Writing("Go", 1).WithShareOfLines(pre: 0, post: 0.5))
            .Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Post.Languages.Single(l => l.Language == "Go").Share.ShouldBe(0.505, tolerance: 1e-9);
    }

    [Fact]
    public void It_counts_as_new()
    {
        // Arrange
        var profile = new GivenAProfile()
            .WithRepo(new GivenARepo("main").Writing("TypeScript", 0.99).Writing("Go", 0.01).WithShareOfLines(pre: 1, post: 0.5))
            .WithRepo(new GivenARepo("svc").Writing("Go", 1).WithShareOfLines(pre: 0, post: 0.5))
            .Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Shift.NewLanguages.ShouldBe(["Go"]);
    }
}
