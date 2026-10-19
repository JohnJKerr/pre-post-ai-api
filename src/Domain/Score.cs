namespace Domain;

public record Score(ScoreParts Parts, string? Tag)
{
    private const double CommitsMax = 28;
    private const double PerNewLanguage = 5;

    public static readonly IReadOnlyList<Rung> Ladder =
    [
        new(0, "I'm still hand-crafting"),
        new(15, "I'm still prompting"),
        new(30, "I've got a pair programmer"),
        new(45, "I run a one-person dev shop"),
        new(65, "I built a software factory"),
        new(85, "I run a lights-out factory"),
    ];

    public double Total => Parts.Total;

    public int Rung => Ladder.Select((rung, index) => (rung, index)).Last(r => Total >= r.rung.Min).index;

    public string Verdict => Ladder[Rung].Verdict;

    // An undefined multiplier (nothing before the cut-off) scores 0 rather than a free maximum.
    // Each part is rounded before adding, so the workings on the report add up exactly. Output weighs
    // commits over lines: a commit is a change someone chose to make, while lines swell with generated code.
    public static Score For(Shift shift)
    {
        var max = ScoreParts.Max;
        var commits = shift.CommitsX is { } commitsX ? RoundedPart(CommitsMax, CommitsMax * Math.Log10(commitsX)) : 0;
        var lines = shift.LinesX is { } linesX ? RoundedPart(max.Output - CommitsMax, (max.Output - CommitsMax) * Math.Log10(linesX)) : 0;
        var projects = shift.ProjectsX is { } projectsX ? RoundedPart(max.Projects, max.Projects * Math.Log2(projectsX) / 2) : 0;
        var languages = Math.Min(max.Languages, PerNewLanguage * shift.NewLanguages.Count);
        var time = shift.ActiveDaysX is { } activeDaysX ? RoundedPart(max.Time, max.Time * Math.Log2(activeDaysX)) : 0;

        return new Score(new ScoreParts(commits + lines, projects, languages, time), TagFor(shift));
    }

    private static double RoundedPart(double max, double value) => Rounding.HalfUp(Math.Clamp(value, 0, max));

    private static string? TagFor(Shift shift) => shift.HoursShift switch
    {
        null => null,
        >= 15 when shift.Post.OutOfHours >= 0.4 => "and it runs nights and weekends",
        <= -10 => "and I got my evenings back",
        _ => "and I still clock off on time",
    };
}
