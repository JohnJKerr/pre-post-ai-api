namespace Domain;

public record WindowActivity(IReadOnlyList<Week> Weeks, IReadOnlyList<PunchCell> PunchCard, int ActiveDays, double PunchShare);
