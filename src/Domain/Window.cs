namespace Domain;

public record Window(DateOnly Start, int Weeks)
{
    public DateOnly End => Start.AddDays(Weeks * 7 - 1);

    public static Window After(DateOnly cutoff, DateOnly today)
    {
        var cutoffWeekStart = WeekStart(cutoff);
        var completeWeeks = (WeekStart(today).DayNumber - cutoffWeekStart.DayNumber) / 7;
        return new Window(cutoffWeekStart, Math.Max(0, completeWeeks));
    }

    public Window Preceding() => this with { Start = Start.AddDays(-Weeks * 7) };

    public static DateOnly WeekStart(DateOnly date) => date.AddDays(-(int)date.DayOfWeek);
}
