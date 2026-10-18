namespace Domain.Tests;

internal class GivenAProfile
{
    private readonly List<Repo> _repos = [];

    public GivenAProfile WithRepo(GivenARepo repo)
    {
        _repos.Add(repo.Build());
        return this;
    }

    public Profile Build() => new(
        "dev",
        IsSample: false,
        new DateTimeOffset(2026, 10, 18, 0, 0, 0, TimeSpan.Zero),
        Cutoff.Default,
        _repos,
        new PerEra<WindowActivity>(
            new WindowActivity([new Week(new DateOnly(2025, 11, 16), 1_000, 100, 10)], [], ActiveDays: 3, PunchShare: 0),
            new WindowActivity([new Week(new DateOnly(2025, 11, 23), 1_000, 100, 10)], [], ActiveDays: 3, PunchShare: 0)));
}
