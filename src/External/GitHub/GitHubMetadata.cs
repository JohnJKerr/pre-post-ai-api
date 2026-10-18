using Domain.GitHub;

namespace External.GitHub;

public class GitHubMetadata : IGitHubMetadata
{
    public Task<GitHubActivity> Fetch(GitHubUser user, DateOnly from, DateOnly to) => throw new NotImplementedException();
}
