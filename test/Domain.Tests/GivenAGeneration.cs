namespace Domain.Tests;

internal class GivenAGeneration
{
    public static readonly DateTimeOffset StartedAt = new(2026, 10, 18, 9, 0, 0, TimeSpan.Zero);
    public static readonly DateTimeOffset FinishedAt = StartedAt.AddSeconds(30);

    public Generation Build() => Generation.Start(gitHubId: 583_231, "octo-dev", Cutoff.Default, StartedAt);
}
