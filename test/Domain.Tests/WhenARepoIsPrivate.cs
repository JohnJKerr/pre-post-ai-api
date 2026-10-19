using Shouldly;

namespace Domain.Tests;

public class WhenARepoIsPrivate
{
    [Fact]
    public void Its_project_is_unnamed()
    {
        // Arrange
        var profile = new GivenTheSampleProfile().WithPrivateRepo("invoice-api").Build();

        // Act
        var report = Report.From(profile);

        // Assert
        report.Pre.Projects.ShouldContain(new Project(null, 17_621));
    }
}
