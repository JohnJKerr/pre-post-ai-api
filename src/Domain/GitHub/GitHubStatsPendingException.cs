namespace Domain.GitHub;

public class GitHubStatsPendingException(string repo)
    : Exception($"GitHub is still computing the stats for {repo}.");
