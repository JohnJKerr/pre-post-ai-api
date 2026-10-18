namespace Domain.Tests;

internal class GivenAShiftSide
{
    private int _added = 10_000;
    private int _commits = 100;
    private int _projectCount = 4;
    private string[] _usedLanguages = ["TypeScript"];
    private double? _outOfHours = 0.2;

    public GivenAShiftSide WithLinesAdded(int added)
    {
        _added = added;
        return this;
    }

    public GivenAShiftSide WithProjects(int projectCount)
    {
        _projectCount = projectCount;
        return this;
    }

    public GivenAShiftSide WithLanguages(params string[] usedLanguages)
    {
        _usedLanguages = usedLanguages;
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
        _usedLanguages = [];
        return this;
    }

    public ShiftSide Build() => new(_added, _commits, _projectCount, _usedLanguages, _outOfHours);
}
