namespace Domain;

// Profiles saved before active hours and merged pull requests were counted read them as 0.
public record WindowActivity(
    IReadOnlyList<Week> Weeks,
    IReadOnlyList<PunchCell> PunchCard,
    int ActiveDays,
    double PunchShare,
    int ActiveHours = 0,
    int MergedPullRequests = 0);
