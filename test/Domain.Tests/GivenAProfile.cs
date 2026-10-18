namespace Domain.Tests;

internal class GivenAProfile
{
    private readonly List<Repo> _repos = [];
    private PunchCell[] _postPunchCard = [];
    private int _addedBefore = 1_000;
    private int _addedAfter = 1_000;

    public GivenAProfile WithRepo(GivenARepo repo)
    {
        _repos.Add(repo.Build());
        return this;
    }

    public GivenAProfile WithPunchCardAfterTheCutoff(params PunchCell[] cells)
    {
        _postPunchCard = cells;
        return this;
    }

    public GivenAProfile WithLinesAdded(int before, int after)
    {
        _addedBefore = before;
        _addedAfter = after;
        return this;
    }

    public Profile Build() => new(
        GitHubId: 1,
        "dev",
        IsSample: false,
        new DateTimeOffset(2026, 10, 18, 0, 0, 0, TimeSpan.Zero),
        Cutoff.Default,
        _repos,
        new PerEra<WindowActivity>(
            new WindowActivity([new Week(new DateOnly(2025, 11, 16), _addedBefore, 0, _addedBefore / 100)], [], ActiveDays: 3, PunchShare: 0),
            new WindowActivity([new Week(new DateOnly(2025, 11, 23), _addedAfter, 0, _addedAfter / 100)], _postPunchCard, ActiveDays: 3, PunchShare: 1)));
}
