using Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(FrontEnd frontEnd) : ControllerBase
{
    [HttpGet("sign-in")]
    public IActionResult SignInWithGitHub([FromQuery] string? returnUrl)
    {
        var path = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        return Challenge(new AuthenticationProperties { RedirectUri = frontEnd.Origin + path }, GitHubSignIn.Scheme);
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> EndSession()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}
