namespace Domain.Tests;

internal class GivenARepo(string name)
{
    private readonly Dictionary<string, double> _languages = [];
    private PerEra<double> _share = new(0, 0);

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

    public Repo Build() => new(name, IsPrivate: false, _languages, _share, PunchCardEra: null, StartedIn: null);
}
