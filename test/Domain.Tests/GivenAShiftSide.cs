namespace Domain.Tests;

internal class GivenAShiftSide
{
    private int _added = 10_000;
    private int _commits = 100;
    private int _projectCount = 4;
    private Dictionary<string, double> _languages = new() { ["TypeScript"] = 1 };
    private int _activeDays = 100;
    private double? _outOfHours = 0.2;

    public GivenAShiftSide WithLinesAdded(int added)
    {
        _added = added;
        return this;
    }

    public GivenAShiftSide WithCommits(int commits)
    {
        _commits = commits;
        return this;
    }

    public GivenAShiftSide WithProjects(int projectCount)
    {
        _projectCount = projectCount;
        return this;
    }

    // Each language takes an equal share.
    public GivenAShiftSide WithLanguages(params string[] languages)
    {
        _languages = languages.ToDictionary(l => l, _ => 1.0 / languages.Length);
        return this;
    }

    public GivenAShiftSide WithLanguage(string language, double share)
    {
        _languages[language] = share;
        return this;
    }

    public GivenAShiftSide WithActiveDays(int activeDays)
    {
        _activeDays = activeDays;
        return this;
    }

    public GivenAShiftSide WithOutOfHours(double share)
    {
        _outOfHours = share;
        return this;
    }

    public GivenAShiftSide WithNoPunchData()
    {
        _outOfHours = null;
        return this;
    }

    public GivenAShiftSide WithNoActivity()
    {
        _added = 0;
        _commits = 0;
        _projectCount = 0;
        _languages = [];
        _activeDays = 0;
        return this;
    }

    public ShiftSide Build() => new(_added, _commits, _projectCount, _languages, _activeDays, _outOfHours);
}
