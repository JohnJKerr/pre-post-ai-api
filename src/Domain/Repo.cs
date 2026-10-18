namespace Domain;

public record Repo(
    string Name,
    bool IsPrivate,
    IReadOnlyDictionary<string, double> Languages,
    PerEra<double> Share,
    Era? PunchCardEra,
    Era? StartedIn);
