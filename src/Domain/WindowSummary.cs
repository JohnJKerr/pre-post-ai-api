namespace Domain;

public record WindowSummary
{
    private const double UsedThreshold = 0.02;
    private static readonly HashSet<string> Markup = ["CSS", "HTML", "SCSS", "Markdown", "MDX", "Dockerfile"];

    public required Era Era { get; init; }
    public required IReadOnlyList<Week> Weekly { get; init; }
    public required int Added { get; init; }
    public required int Deleted { get; init; }
    public required int Commits { get; init; }
    public required IReadOnlyList<Project> Projects { get; init; }
    public required int Started { get; init; }
    public required int ActiveDays { get; init; }
    public required IReadOnlyList<LanguageShare> Languages { get; init; }
    public required Punch? Punch { get; init; }
    public required Coverage Coverage { get; init; }

    public int Weeks => Weekly.Count;
    public Window Range => new(Weekly[0].Start, Weeks);
    public int PerCommit => Commits > 0 ? (int)Rounding.HalfUp((double)Added / Commits) : 0;
    public IReadOnlyList<string> UsedLanguages => Languages.Where(l => l.Share >= UsedThreshold).Select(l => l.Language).ToArray();
    public double ActiveDaysPerWeek => Weeks > 0 ? Math.Round((double)ActiveDays / Weeks, 1, MidpointRounding.AwayFromZero) : 0;

    public int ProjectCount => Projects.Count;

    public static WindowSummary For(Profile profile, Era era)
    {
        var activity = profile.Activity[era];
        var weeks = activity.Weeks;
        var added = weeks.Sum(w => w.Added);
        var projects = ProjectsIn(profile.Repos, era, added);
        var eligibleRepos = profile.Repos.Count(r => r.PunchCardEra == era);
        var punch = eligibleRepos > 0 ? Punch.From(activity.PunchCard) : null;

        return new WindowSummary
        {
            Era = era,
            Weekly = weeks,
            Added = added,
            Deleted = weeks.Sum(w => w.Deleted),
            Commits = weeks.Sum(w => w.Commits),
            Projects = projects,
            Started = profile.Repos.Count(r => r.StartedIn == era),
            ActiveDays = activity.ActiveDays,
            Languages = LanguagesIn(profile.Repos, era),
            Punch = punch,
            Coverage = new Coverage(eligibleRepos, projects.Count, punch is null ? 0 : activity.PunchShare),
        };
    }

    private static IReadOnlyList<Project> ProjectsIn(IReadOnlyList<Repo> repos, Era era, int added) =>
        repos
            .Where(r => r.Share[era] > 0)
            .Select(r => new Project(r.IsPrivate ? null : r.Name, (int)Rounding.HalfUp(r.Share[era] * added)))
            .OrderByDescending(p => p.Lines)
            .ToArray();

    private static IReadOnlyList<LanguageShare> LanguagesIn(IReadOnlyList<Repo> repos, Era era)
    {
        var weighted = new Dictionary<string, double>();
        foreach (var repo in repos.Where(r => r.Share[era] > 0))
            foreach (var (language, share) in repo.Languages.Where(l => !Markup.Contains(l.Key)))
                weighted[language] = weighted.GetValueOrDefault(language) + share * repo.Share[era];

        var total = weighted.Values.Sum();
        if (total == 0) return [];

        return weighted
            .Select(l => new LanguageShare(l.Key, l.Value / total))
            .OrderByDescending(l => l.Share)
            .ToArray();
    }
}
