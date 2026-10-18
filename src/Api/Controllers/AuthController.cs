using Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpGet("sign-in")]
    public IActionResult SignIn([FromQuery] string? returnUrl)
    {
        var redirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, GitHubSignIn.Scheme);
    }
}
