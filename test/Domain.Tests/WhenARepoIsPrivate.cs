using Shouldly;

namespace Domain.Tests;

public class WhenARepoIsPrivate
{
    private readonly Report _report = Report.From(new GivenTheSampleProfile().WithPrivateRepo("invoice-api").Build());

    [Fact]
    public void Its_project_is_unnamed() => _report.Pre.Projects.ShouldContain(new Project(null, 18_611));
}
