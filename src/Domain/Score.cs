namespace Domain;

public record Score(ScoreParts Parts)
{
    private const double CommitsMax = 28;
    private const double PerNewLanguage = 5;

    public static readonly IReadOnlyList<Rung> Ladder =
    [
        new(0, "I'm an artisan"),
        new(15, "I've hired an apprentice"),
        new(30, "I've grown robot arms"),
        new(45, "I'm a cyborg"),
        new(65, "I've been assimilated"),
        new(85, "I'm a machine (literally)"),
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

        return new Score(new ScoreParts(commits + lines, projects, languages, time));
    }

    private static double RoundedPart(double max, double value) => Rounding.HalfUp(Math.Clamp(value, 0, max));
}
