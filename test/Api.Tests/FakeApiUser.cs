using Api;
using Domain.GitHub;

namespace Api.Tests;

internal class FakeApiUser(GitHubUser? user) : IApiUser
{
    public static FakeApiUser SignedInAs(string handle) => new(new GitHubUser(handle, "user-token"));

    public static FakeApiUser SignedOut() => new(null);

    public GitHubUser? User => user;
}
