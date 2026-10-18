namespace Domain;

public record Punch(IReadOnlyList<PunchCell> Cells)
{
    private const int Sunday = 0;
    private const int Saturday = 6;

    private int Total => Cells.Sum(c => c.Commits);

    public PunchCell Busiest => Cells.Aggregate((best, cell) => cell.Commits > best.Commits ? cell : best);

    public IReadOnlyList<HourShare> HourShares => Enumerable.Range(0, 24)
        .Select(hour => new HourShare(hour, (double)Cells.Where(c => c.Hour == hour).Sum(c => c.Commits) / Total))
        .ToArray();

    public CodingWindow CodingWindow => CodingWindow.Holding80PercentOf(HourShares);

    public int HoursTouched => HourShares.Count(h => h.Share > 0);

    public double Evening => ShareOf(IsEvening);

    public double Weekend => ShareOf(IsWeekend);

    public double Combined => ShareOf(c => IsEvening(c) || IsWeekend(c));

    public static Punch? From(IReadOnlyList<PunchCell> cells) => cells.Sum(c => c.Commits) > 0 ? new Punch(cells) : null;

    private static bool IsEvening(PunchCell cell) => cell.Day is > Sunday and < Saturday && (cell.Hour >= 18 || cell.Hour < 6);

    private static bool IsWeekend(PunchCell cell) => cell.Day is Sunday or Saturday;

    private double ShareOf(Func<PunchCell, bool> cells) => (double)Cells.Where(cells).Sum(c => c.Commits) / Total;
}
