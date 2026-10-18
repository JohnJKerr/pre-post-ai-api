namespace Domain;

public record Scales(int WeekMax, int ProjectMax, int CellMax, double HourMax)
{
    private const double HourHeadroom = 1.08;

    public static Scales Across(WindowSummary pre, WindowSummary post)
    {
        WindowSummary[] both = [pre, post];
        var punches = both.Select(s => s.Punch).OfType<Punch>().ToArray();

        return new Scales(
            WeekMax: AtLeastOne(both.SelectMany(s => s.Weekly).Select(w => w.Added)),
            ProjectMax: AtLeastOne(both.SelectMany(s => s.Projects).Select(p => p.Lines)),
            CellMax: AtLeastOne(punches.SelectMany(p => p.Cells).Select(c => c.Commits)),
            HourMax: punches.SelectMany(p => p.HourShares).Select(h => h.Share).Append(0.01).Max() * HourHeadroom);
    }

    private static int AtLeastOne(IEnumerable<int> values) => values.Append(1).Max();
}
