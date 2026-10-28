using System.Diagnostics;
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
            var generation = await Generate(request);
            logger.LogInformation("Generation {GenerationId} for {Handle} finished {Status} in {Seconds:0.0} s.",
                generation.Id, generation.Handle, generation.Status, generation.Took?.TotalSeconds);
            await generations.Save(generation);
        }
    }

    private async Task<Generation> Generate(GenerationRequest request)
    {
        var (generation, user) = request;
        var fetchMs = 0d;
        var assemblyMs = 0d;
        var saveMs = 0d;
        try
        {
            var now = time.GetUtcNow();
            var windows = Window.Around(generation.Cutoff.Date, DateOnly.FromDateTime(now.UtcDateTime));
            using var scope = scopes.CreateScope();
            var started = Stopwatch.GetTimestamp();
            GitHubActivity activity;
            try { activity = await scope.ServiceProvider.GetRequiredService<IGitHubMetadata>().Fetch(user, windows); }
            finally { fetchMs = Stopwatch.GetElapsedTime(started).TotalMilliseconds; }

            started = Stopwatch.GetTimestamp();
            Profile profile;
            try { profile = Profile.Assemble(user.Id, user.Handle, generation.Cutoff, activity, now); }
            finally { assemblyMs = Stopwatch.GetElapsedTime(started).TotalMilliseconds; }
            if (!profile.HasCommits) return generation.FindNothingToCompare(time.GetUtcNow());

            started = Stopwatch.GetTimestamp();
            try { await profiles.Save(profile); }
            finally { saveMs = Stopwatch.GetElapsedTime(started).TotalMilliseconds; }
            return generation.Complete(time.GetUtcNow());
        }
        catch (GitHubStatsPendingException)
        {
            return generation.Fail("GitHub is still crunching your repository stats. Sign in again in a minute to retry.", time.GetUtcNow());
        }
        catch (GitHubTokenRejectedException)
        {
            return generation.Fail("GitHub didn't accept your sign-in. Sign in again.", time.GetUtcNow());
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Generation {GenerationId} for {Handle} failed.", generation.Id, generation.Handle);
            return generation.Fail("Something went wrong generating your report. Sign in to try again.", time.GetUtcNow());
        }
        finally
        {
            logger.LogInformation("Generation {GenerationId} phases: fetch {FetchMs:0} ms, assembly {AssemblyMs:0} ms, save {SaveMs:0} ms.",
                generation.Id, fetchMs, assemblyMs, saveMs);
        }
    }
}
