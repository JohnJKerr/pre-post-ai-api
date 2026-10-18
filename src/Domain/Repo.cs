namespace Domain;

// Name is null except in the sample, whose repos are made up. Real repo names are never kept.
public record Repo(
    string? Name,
    bool IsPrivate,
    IReadOnlyDictionary<string, double> Languages,
    PerEra<double> Share,
    Era? PunchCardEra,
    Era? StartedIn);
