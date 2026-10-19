namespace Domain;

public record ShiftSide(
    int Added,
    int Commits,
    int ProjectCount,
    IReadOnlyDictionary<string, double> Languages,
    int ActiveDays,
    double? OutOfHours);
