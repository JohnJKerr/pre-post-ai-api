using Shouldly;

namespace Domain.Tests;

public class WhenAWindowHasNoPunchCardEligibleRepos
{
    private readonly Report _report = Report.From(new GivenTheSampleProfile().WithNoPunchCardEligibleReposBeforeTheCutoff().Build());

    [Fact]
    public void The_punch_card_is_hidden() => _report.Pre.Punch.ShouldBeNull();

    [Fact]
    public void Coverage_is_empty() => _report.Pre.Coverage.ShouldBe(new Coverage(Eligible: 0, Total: 5, Share: 0));

    [Fact]
    public void The_out_of_hours_multiplier_is_undefined() => _report.Shift.OutOfHoursX.ShouldBeNull();

    [Fact]
    public void Hours_are_left_out_of_the_score() => _report.Score.Parts.Hours.ShouldBe(0);

    [Fact]
    public void There_is_no_tag() => _report.Score.Tag.ShouldBeNull();
}
