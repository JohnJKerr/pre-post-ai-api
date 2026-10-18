namespace Domain;

// One run of gathering a user's GitHub metadata into their profile. It runs in the background after
// sign-in, and the front end polls it by its unguessable id until it's ready or has failed.
public record Generation(
    string Id,
    long GitHubId,
    string Handle,
    Cutoff Cutoff,
    DateTimeOffset StartedAt,
    GenerationStatus Status,
    string? Message)
{
    // Long enough for a large account whose stats GitHub is still computing. A generation still
    // pending after this was lost, most likely to a restart, since the job holds the token only in memory.
    private static readonly TimeSpan Abandoned = TimeSpan.FromMinutes(30);

    public static Generation Start(long gitHubId, string handle, Cutoff cutoff, DateTimeOffset now) =>
        new(Guid.NewGuid().ToString("N"), gitHubId, handle, cutoff, now, GenerationStatus.Pending, Message: null);

    public Generation Complete() => this with { Status = GenerationStatus.Ready };

    public Generation Fail(string message) => this with { Status = GenerationStatus.Failed, Message = message };

    public Generation AsOf(DateTimeOffset now) =>
        Status == GenerationStatus.Pending && now - StartedAt > Abandoned
            ? Fail("Something interrupted your report. Sign in to try again.")
            : this;
}
