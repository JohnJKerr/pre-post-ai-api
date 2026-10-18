namespace Domain;

public record ShiftSide(int Added, int Commits, int ProjectCount, IReadOnlyList<string> UsedLanguages, double? OutOfHours);
