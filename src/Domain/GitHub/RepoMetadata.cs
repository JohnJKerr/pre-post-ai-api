namespace Domain.GitHub;

// No name: the report doesn't show repo names, so they never leave the GitHub adapter.
public record RepoMetadata(
    bool IsPrivate,
    DateOnly CreatedOn,
    IReadOnlyDictionary<string, long> LanguageBytes,
    IReadOnlyList<Week> AuthorWeeks,
    int TotalCommits,
    IReadOnlyList<PunchCell> PunchCard);
