using Shouldly;

namespace Domain.Tests;

public class WhenThereIsNoActivityBeforeTheCutoff
{
    [Fact]
    public void Pre_adds_0_lines_per_commit()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().WithNoActivityBeforeTheCutoff().Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.PerCommit.ShouldBe(0);
    }
}
