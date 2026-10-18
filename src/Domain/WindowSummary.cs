namespace Domain;

public record WindowSummary
{
    public required Era Era { get; init; }
    public required IReadOnlyList<Week> Weekly { get; init; }
    public required int Added { get; init; }
    public required int Deleted { get; init; }
    public required int Commits { get; init; }
    public required IReadOnlyList<Project> Projects { get; init; }
    public required int Started { get; init; }
    public required int ActiveDays { get; init; }

    public int Weeks => Weekly.Count;
    public Window Range => new(Weekly[0].Start, Weeks);
    public int PerCommit => Commits > 0 ? (int)Rounding.HalfUp((double)Added / Commits) : 0;
    public double ActiveDaysPerWeek => Weeks > 0 ? Math.Round((double)ActiveDays / Weeks, 1, MidpointRounding.AwayFromZero) : 0;

    public int ProjectCount => Projects.Count;

    public static WindowSummary For(Profile profile, Era era)
    {
        var activity = profile.Activity[era];
        var weeks = activity.Weeks;
        var added = weeks.Sum(w => w.Added);

        return new WindowSummary
        {
            Era = era,
            Weekly = weeks,
            Added = added,
            Deleted = weeks.Sum(w => w.Deleted),
            Commits = weeks.Sum(w => w.Commits),
            Projects = ProjectsIn(profile.Repos, era, added),
            Started = profile.Repos.Count(r => r.StartedIn == era),
            ActiveDays = activity.ActiveDays,
        };
    }

    private static IReadOnlyList<Project> ProjectsIn(IReadOnlyList<Repo> repos, Era era, int added) =>
        repos
            .Where(r => r.Share[era] > 0)
            .Select(r => new Project(r.IsPrivate ? null : r.Name, (int)Rounding.HalfUp(r.Share[era] * added)))
            .OrderByDescending(p => p.Lines)
            .ToArray();
}
