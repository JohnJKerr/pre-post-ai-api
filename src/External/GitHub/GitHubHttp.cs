using System.Net;

namespace External.GitHub;

public static class GitHubHttp
{
    // The handler sends Accept-Encoding and inflates the body. The adapter reads the JSON as it arrives.
    public static SocketsHttpHandler PrimaryHandler() =>
        new() { AutomaticDecompression = DecompressionMethods.All };
}
