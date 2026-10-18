using System.Globalization;
using System.Security.Claims;
using Domain.GitHub;

namespace Api;

public class ApiUser(IHttpContextAccessor httpContextAccessor) : IApiUser
{
    public const string GitHubIdClaim = "github_id";
    public const string AccessTokenClaim = "github_access_token";

    public GitHubUser? User
    {
        get
        {
            var principal = httpContextAccessor.HttpContext?.User;
            var id = principal?.FindFirstValue(GitHubIdClaim);
            var handle = principal?.FindFirstValue(ClaimTypes.Name);
            var accessToken = principal?.FindFirstValue(AccessTokenClaim);
            return id is null || handle is null || accessToken is null
                ? null
                : new GitHubUser(long.Parse(id, CultureInfo.InvariantCulture), handle, accessToken);
        }
    }
}
