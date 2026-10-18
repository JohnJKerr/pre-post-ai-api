using Domain.GitHub;

namespace Api;

public interface IApiUser
{
    GitHubUser? User { get; }
}
