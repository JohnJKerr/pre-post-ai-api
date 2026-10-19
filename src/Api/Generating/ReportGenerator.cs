using Domain;
using Domain.GitHub;
using Domain.Repositories;

namespace Api.Generating;

// Gathers each signed-in user's GitHub metadata into their profile, outside the sign-in request.
// GitHub's secondary rate limits are per token, so a few users' jobs run at once while each job
// makes its own calls one after another.
public class ReportGenerator(
    GenerationQueue queue,
    IServiceScopeFactory scopes,
    IProfileRepository profiles,
    IGenerationRepository generations,
    TimeProvider time,
    ILogger<ReportGenerator> logger) : BackgroundService
{
    private const int Workers = 4;

    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.WhenAll(Enumerable.Range(0, Workers).Select(_ => Work(stoppingToken)));

    private async Task Work(CancellationToken stoppingToken)
    {
        await foreach (var request in queue.Requests(stoppingToken))
        {
            await generations.Save(await Generate(request));
        }
    }

    private async Task<Generation> Generate(GenerationRequest request)
    {
        var (generation, user) = request;
        try
        {
            var now = time.GetUtcNow();
            var post = Window.After(generation.Cutoff.Date, DateOnly.FromDateTime(now.UtcDateTime));
            using var scope = scopes.CreateScope();
            var activity = await scope.ServiceProvider.GetRequiredService<IGitHubMetadata>().Fetch(user, post.Preceding().Start, post.End);

            var profile = Profile.Assemble(user.Id, user.Handle, generation.Cutoff, activity, now);
            if (!profile.HasCommits) return generation.FindNothingToCompare();

            await profiles.Save(profile);
            return generation.Complete();
        }
        catch (GitHubTokenRejectedException)
        {
            return generation.Fail("GitHub didn't accept your sign-in. Sign in again.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Generation {GenerationId} for {Handle} failed.", generation.Id, generation.Handle);
            return generation.Fail("Something went wrong generating your report. Sign in to try again.");
        }
    }
}
