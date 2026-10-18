using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/session")]
public class SessionController(IApiUser apiUser) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => apiUser.User is { } user
        ? Ok(new SessionResponse(user.Handle))
        : Unauthorized(new ErrorResponse("You're not signed in."));
}
