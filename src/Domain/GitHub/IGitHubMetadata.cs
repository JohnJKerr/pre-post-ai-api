namespace Domain.GitHub;

public interface IGitHubMetadata
{
    Task<GitHubActivity> Fetch(GitHubUser user, DateOnly from, DateOnly to);
}
