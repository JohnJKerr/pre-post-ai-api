namespace Domain.GitHub;

// GitHub answered 401: the user's token has expired or been revoked, so they must sign in again.
public class GitHubTokenRejectedException() : Exception("GitHub rejected the user's access token.");
