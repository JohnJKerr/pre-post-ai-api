namespace Domain;

public record Cutoff(string Label, DateOnly Date)
{
    public static readonly Cutoff Default = new("Claude Opus 4.5", new DateOnly(2025, 11, 24));

    private static readonly Cutoff[] Named = [Default, new("ChatGPT launch", new DateOnly(2022, 11, 30))];

    public static Cutoff On(DateOnly date) => Named.FirstOrDefault(c => c.Date == date) ?? new Cutoff("Custom", date);
}
