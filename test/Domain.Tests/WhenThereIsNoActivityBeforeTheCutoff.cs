using Shouldly;

namespace Domain.Tests;

public class WhenThereIsNoActivityBeforeTheCutoff
{
    private readonly Report _report = Report.From(new GivenTheSampleProfile().WithNoActivityBeforeTheCutoff().Build());

    [Fact]
    public void Pre_adds_0_lines_per_commit() => _report.Pre.PerCommit.ShouldBe(0);
}
