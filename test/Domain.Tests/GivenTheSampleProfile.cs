namespace Domain.Tests;

internal class GivenTheSampleProfile
{
    private Profile _profile = SampleProfile.Generate();

    public GivenTheSampleProfile WithPrivateRepo(string name)
    {
        var repos = _profile.Repos.Select(r => r.Name == name ? r with { IsPrivate = true } : r).ToArray();
        _profile = _profile with { Repos = repos };
        return this;
    }

    public GivenTheSampleProfile WithNoActivityBeforeTheCutoff()
    {
        var pre = _profile.Activity.Pre;
        var emptyWeeks = pre.Weeks.Select(w => w with { Added = 0, Deleted = 0, Commits = 0 }).ToArray();
        _profile = _profile with { Activity = _profile.Activity with { Pre = pre with { Weeks = emptyWeeks } } };
        return this;
    }

    public Profile Build() => _profile;
}
