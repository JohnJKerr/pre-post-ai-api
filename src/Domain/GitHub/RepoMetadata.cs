namespace Domain.GitHub;

public record RepoMetadata(
    string Name,
    bool IsPrivate,
    DateOnly CreatedOn,
    IReadOnlyDictionary<string, long> LanguageBytes,
    IReadOnlyList<Week> AuthorWeeks,
    int TotalCommits,
    IReadOnlyList<PunchCell> PunchCard);
