namespace Domain;

public record Shift(ShiftSide Pre, ShiftSide Post)
{
    public double? LinesX => Ratio(Post.Added, Pre.Added);

    public double? ProjectsX => Ratio(Post.ProjectCount, Pre.ProjectCount);

    public double? CommitsX => Ratio(Post.Commits, Pre.Commits);

    public IReadOnlyList<string> NewLanguages => Post.UsedLanguages.Except(Pre.UsedLanguages).ToArray();

    // Rounded to 1e-6 points so float noise can't push a shift across the -10 / +15 thresholds.
    public double? HoursShift => Pre.OutOfHours is { } pre && Post.OutOfHours is { } post
        ? Rounding.HalfUp((post - pre) * 1e8) / 1e6
        : null;

    public Score Score() => Domain.Score.For(this);

    private static double? Ratio(double post, double pre) => pre > 0 ? post / pre : null;
}
