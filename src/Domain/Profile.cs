namespace Domain;

public record Profile(
    string Handle,
    bool IsSample,
    DateTimeOffset GeneratedAt,
    Cutoff Cutoff,
    IReadOnlyList<Repo> Repos,
    PerEra<WindowActivity> Activity);
