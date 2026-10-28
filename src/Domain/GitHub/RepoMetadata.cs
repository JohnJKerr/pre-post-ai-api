namespace Domain.GitHub;

// No name: the report doesn't show repo names, so they never leave the GitHub adapter.
public record RepoMetadata(
    bool IsPrivate,
    DateOnly CreatedOn,
    IReadOnlyDictionary<string, long> LanguageBytes,
    IReadOnlyList<Week> AuthorWeeks,
    int TotalCommits,
    IReadOnlyList<PunchCell> PunchCard)
{
    public Era? PunchCardEra(PerEra<Window> windows)
    {
        var authored = AuthorWeeks.Sum(w => w.Commits);
        if (TotalCommits == 0 || (double)authored / TotalCommits < 0.9) return null;

        var activeWeeks = AuthorWeeks.Where(w => w.Commits > 0).ToArray();
        var cutoffWeekStart = windows.Pre.End.AddDays(1);
        Era? side = activeWeeks.All(w => w.Start < cutoffWeekStart) ? Era.Pre
            : activeWeeks.All(w => w.Start >= cutoffWeekStart) ? Era.Post
            : null;

        return side is { } era && AuthorWeeks.Where(w => windows[era].Contains(w.Start)).Sum(w => w.Commits) > 0
            ? era : null;
    }
}
