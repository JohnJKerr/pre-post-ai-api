using Shouldly;

namespace Domain.Tests;

public class WhenAWindowHasNoPunchCardEligibleRepos
{
    [Fact]
    public void The_punch_card_is_hidden()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().WithNoPunchCardEligibleReposBeforeTheCutoff().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Punch.ShouldBeNull();
    }

    [Fact]
    public void Coverage_is_empty()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().WithNoPunchCardEligibleReposBeforeTheCutoff().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Coverage.ShouldBe(new Coverage(Eligible: 0, Total: 5, Share: 0));
    }

    [Fact]
    public void The_out_of_hours_multiplier_is_undefined()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().WithNoPunchCardEligibleReposBeforeTheCutoff().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Shift.OutOfHoursX.ShouldBeNull();
    }

    [Fact]
    public void There_is_no_tag()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().WithNoPunchCardEligibleReposBeforeTheCutoff().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Score.Tag.ShouldBeNull();
    }
}
