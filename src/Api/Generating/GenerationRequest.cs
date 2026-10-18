using Domain;
using Domain.GitHub;

namespace Api.Generating;

// Carries the user's GitHub token from the sign-in callback to the job. It lives only in memory and
// is gone once the job finishes.
public record GenerationRequest(Generation Generation, GitHubUser User);
