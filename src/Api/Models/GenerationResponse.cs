using Domain;

namespace Api.Models;

// Carries the cut-off so "Sign in again" can reuse it from any tab.
public record GenerationResponse(string Status, CutoffResponse Cutoff, string? Handle, string? Message)
{
    public static GenerationResponse From(Generation generation) => new(
        generation.Status.ToString().ToLowerInvariant(),
        new CutoffResponse(generation.Cutoff.Label, Dates.Format(generation.Cutoff.Date)),
        generation.Status == GenerationStatus.Ready ? generation.Handle : null,
        generation.Message);
}
