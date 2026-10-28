namespace Domain.GitHub;

public interface IGitHubMetadata
{
    Task<GitHubActivity> Fetch(GitHubUser user, PerEra<Window> windows);
}
