namespace Domain.Tests;

internal class GivenARepo(string name)
{
    private readonly Dictionary<string, double> _languages = [];
    private PerEra<double> _share = new(0, 0);
    private Era? _punchCardEra;

    public GivenARepo Writing(string language, double share)
    {
        _languages[language] = share;
        return this;
    }

    public GivenARepo WithShareOfLines(double pre, double post)
    {
        _share = new PerEra<double>(pre, post);
        return this;
    }

    public GivenARepo WithPunchCardFor(Era era)
    {
        _punchCardEra = era;
        return this;
    }

    public Repo Build() => new(name, IsPrivate: false, _languages, _share, _punchCardEra, StartedIn: null);
}
