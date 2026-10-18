using Domain;

namespace Api.Models;

public record GenerationResponse(string Status, string? Handle, string? Message)
{
    public static GenerationResponse From(Generation generation) => new(
        generation.Status.ToString().ToLowerInvariant(),
        generation.Status == GenerationStatus.Ready ? generation.Handle : null,
        generation.Message);
}
