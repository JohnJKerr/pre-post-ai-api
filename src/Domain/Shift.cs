namespace Domain;

public record Shift(ShiftSide Pre, ShiftSide Post)
{
    private const double NewLanguageMin = 0.05;
    private const double PreviousLanguageMax = 0.01;

    public double? LinesX => Ratio(Post.Added, Pre.Added);

    public double? ProjectsX => Ratio(Post.ProjectCount, Pre.ProjectCount);

    public double? CommitsX => Ratio(Post.Commits, Pre.Commits);

    public double? OutOfHoursX => Pre.OutOfHours is { } pre && Post.OutOfHours is { } post ? Ratio(post, pre) : null;

    public double? ActiveDaysX => Ratio(Post.ActiveDays, Pre.ActiveDays);

    // New means a real part of the work after the cut-off that was next to nothing before it, so a
    // language hovering around a threshold can't flip between windows.
    public IReadOnlyList<string> NewLanguages => Post.Languages
        .Where(l => l.Value >= NewLanguageMin && Pre.Languages.GetValueOrDefault(l.Key) <= PreviousLanguageMax)
        .OrderByDescending(l => l.Value)
        .Select(l => l.Key)
        .ToArray();

    // Rounded to 1e-6 points so float noise can't push a shift across the -10 / +15 thresholds.
    public double? HoursShift => Pre.OutOfHours is { } pre && Post.OutOfHours is { } post
        ? Rounding.HalfUp((post - pre) * 1e8) / 1e6
        : null;

    public Score Score() => Domain.Score.For(this);

    public static Shift Between(WindowSummary pre, WindowSummary post) => new(SideOf(pre), SideOf(post));

    private static ShiftSide SideOf(WindowSummary summary) =>
        new(
            summary.Added,
            summary.Commits,
            summary.ProjectCount,
            summary.Languages.ToDictionary(l => l.Language, l => l.Share),
            summary.ActiveDays,
            summary.Punch?.Combined);

    private static double? Ratio(double post, double pre) => pre > 0 ? post / pre : null;
}
