using Domain;

namespace Api.Tests;

internal class GivenAProfile
{
    private Profile _profile = SampleProfile.Generate() with { GitHubId = 1_001, IsSample = false };

    public GivenAProfile For(string handle)
    {
        _profile = _profile with { Handle = handle };
        return this;
    }

    public Profile Build() => _profile;
}
