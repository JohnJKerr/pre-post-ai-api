using Shouldly;

namespace Domain.Tests;

public class WhenAWindowHasNoPunchCardEligibleRepos
{
    private readonly Report _report = Report.From(new GivenTheSampleProfile().WithNoPunchCardEligibleReposBeforeTheCutoff().Build());

    [Fact]
    public void The_punch_card_is_hidden() => _report.Pre.Punch.ShouldBeNull();

    [Fact]
    public void Coverage_is_empty() => _report.Pre.Coverage.ShouldBe(new Coverage(Eligible: 0, Total: 5, Share: 0));
}
