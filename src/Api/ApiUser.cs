using System.Security.Claims;
using Domain.GitHub;

namespace Api;

public class ApiUser(IHttpContextAccessor httpContextAccessor) : IApiUser
{
    public const string AccessTokenClaim = "github_access_token";

    public GitHubUser? User
    {
        get
        {
            var principal = httpContextAccessor.HttpContext?.User;
            var handle = principal?.FindFirstValue(ClaimTypes.Name);
            var accessToken = principal?.FindFirstValue(AccessTokenClaim);
            return handle is null || accessToken is null ? null : new GitHubUser(handle, accessToken);
        }
    }
}
