namespace Domain;

// The front end names cut-offs; the API works from the date alone.
public record Cutoff(DateOnly Date)
{
    public static readonly Cutoff Default = new(new DateOnly(2025, 11, 24));
}
