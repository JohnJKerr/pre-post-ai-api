namespace Domain;

public record Cutoff(string Label, DateOnly Date)
{
    public static readonly Cutoff Default = new("Claude Opus 4.5", new DateOnly(2025, 11, 24));
}
