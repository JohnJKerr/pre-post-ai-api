namespace External.GitHub;

public record GitHubOptions(TimeSpan StatsRetryDelay, int StatsAttempts)
{
    public static readonly GitHubOptions Default = new(TimeSpan.FromSeconds(2), StatsAttempts: 15);
    public static readonly GitHubOptions NoDelay = Default with { StatsRetryDelay = TimeSpan.Zero };
}
