namespace Domain;

public record Windows(DateOnly CutoffWeekStart, int Weeks, DateRange Pre, DateRange Post)
{
    public static Windows For(DateOnly cutoff, DateOnly today)
    {
        var cutoffWeekStart = WeekStart(cutoff);
        var daysToCurrentWeek = WeekStart(today).DayNumber - cutoffWeekStart.DayNumber;
        var weeks = Math.Max(0, daysToCurrentWeek / 7);
        var span = weeks * 7;

        return new Windows(
            cutoffWeekStart,
            weeks,
            new DateRange(cutoffWeekStart.AddDays(-span), cutoffWeekStart.AddDays(-1)),
            new DateRange(cutoffWeekStart, cutoffWeekStart.AddDays(span - 1)));
    }

    public static DateOnly WeekStart(DateOnly date) => date.AddDays(-(int)date.DayOfWeek);
}
