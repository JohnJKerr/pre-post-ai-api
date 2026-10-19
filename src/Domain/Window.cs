namespace Domain;

public record Window(DateOnly Start, int Weeks)
{
    public const int MaxWeeks = 26;

    public DateOnly End => Start.AddDays(Weeks * 7 - 1);

    public IEnumerable<DateOnly> WeekStarts => Enumerable.Range(0, Weeks).Select(k => Start.AddDays(k * 7));

    public static Window After(DateOnly cutoff, DateOnly today)
    {
        var cutoffWeekStart = WeekStart(cutoff);
        var completeWeeks = (WeekStart(today).DayNumber - cutoffWeekStart.DayNumber) / 7;
        return new Window(cutoffWeekStart, Math.Max(0, completeWeeks));
    }

    // Pre is the weeks just before the cut-off and post the latest complete weeks, at most 26 of each,
    // so an old cut-off doesn't stretch either window over years.
    public static PerEra<Window> Around(DateOnly cutoff, DateOnly today)
    {
        var weeks = Math.Min(MaxWeeks, After(cutoff, today).Weeks);
        return new PerEra<Window>(
            new Window(WeekStart(cutoff).AddDays(-weeks * 7), weeks),
            new Window(WeekStart(today).AddDays(-weeks * 7), weeks));
    }

    public bool Contains(DateOnly date) => date >= Start && date <= End;

    public static DateOnly WeekStart(DateOnly date) => date.AddDays(-(int)date.DayOfWeek);
}
