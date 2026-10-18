using Api;
using Domain.GitHub;

namespace Api.Tests;

internal class FakeApiUser(GitHubUser? user) : IApiUser
{
    // Each handle gets its own GitHub id unless a test gives one, so reports for different handles
    // never replace each other.
    public static FakeApiUser SignedInAs(string handle, long? gitHubId = null) =>
        new(new GitHubUser(gitHubId ?? IdFor(handle), handle, "user-token"));

    public static FakeApiUser SignedOut() => new(null);

    public GitHubUser? User => user;

    private static long IdFor(string handle) => handle.ToLowerInvariant().Aggregate(17L, (id, c) => id * 31 + c);
}
