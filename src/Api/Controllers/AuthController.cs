using System.Globalization;
using Api.Authentication;
using Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(FrontEnd frontEnd, TimeProvider time) : ControllerBase
{
    // A browser navigation rather than a fetch, so a bad cut-off goes back to the front end as an
    // error code it can explain, not as a JSON body.
    [HttpGet("sign-in")]
    public IActionResult SignInWithGitHub([FromQuery] string? cutoff)
    {
        var date = Cutoff.Default.Date;
        if (cutoff is not null && !DateOnly.TryParseExact(cutoff, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            return Redirect($"{frontEnd.Origin}/generate?error=invalid-cutoff");
        }

        if (Window.After(date, DateOnly.FromDateTime(time.GetUtcNow().UtcDateTime)).Weeks == 0)
        {
            return Redirect($"{frontEnd.Origin}/generate?error=cutoff-too-recent");
        }

        var properties = new AuthenticationProperties();
        properties.Items[GitHubSignIn.CutoffItem] = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return Challenge(properties, GitHubSignIn.Scheme);
    }
}
