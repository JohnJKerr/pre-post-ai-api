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

    public Profile Build() => _profile;
}
