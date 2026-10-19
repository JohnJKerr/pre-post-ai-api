namespace External.GitHub;

public record GitHubOptions(TimeSpan StatsRetryDelay, TimeSpan StatsTimeout)
{
    public static readonly GitHubOptions Default = new(TimeSpan.FromSeconds(5), StatsTimeout: TimeSpan.FromMinutes(5));
    public static readonly GitHubOptions NoDelay = Default with { StatsRetryDelay = TimeSpan.Zero };
}
