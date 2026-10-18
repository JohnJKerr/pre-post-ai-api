using Shouldly;

namespace Domain.Tests;

public class WhenBuildingTheSampleReport
{
    private readonly Report _report = Report.From(new GivenTheSampleProfile().Build());

    [Fact]
    public void Pre_adds_40_459_lines() => _report.Pre.Added.ShouldBe(40_459);

    [Fact]
    public void Pre_deletes_12_665_lines() => _report.Pre.Deleted.ShouldBe(12_665);

    [Fact]
    public void Pre_has_638_commits() => _report.Pre.Commits.ShouldBe(638);

    [Fact]
    public void Post_adds_226_832_lines() => _report.Post.Added.ShouldBe(226_832);

    [Fact]
    public void Post_deletes_74_833_lines() => _report.Post.Deleted.ShouldBe(74_833);

    [Fact]
    public void Post_has_1_604_commits() => _report.Post.Commits.ShouldBe(1_604);

    [Fact]
    public void Pre_has_5_projects() => _report.Pre.ProjectCount.ShouldBe(5);

    [Fact]
    public void Pre_started_2_projects() => _report.Pre.Started.ShouldBe(2);

    [Fact]
    public void Post_has_12_projects() => _report.Post.ProjectCount.ShouldBe(12);

    [Fact]
    public void Post_started_10_projects() => _report.Post.Started.ShouldBe(10);

    [Fact]
    public void Projects_are_ordered_by_lines_descending() =>
        _report.Post.Projects.Select(p => p.Lines).ShouldBeInOrder(SortDirection.Descending);

    [Fact]
    public void A_project_gets_its_share_of_the_window_s_lines() =>
        _report.Pre.Projects.ShouldContain(new Project("invoice-api", 18_611));
}
