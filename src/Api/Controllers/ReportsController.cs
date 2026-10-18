using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    [HttpGet("{handle}")]
    public IActionResult Get(string handle) => NotFound(new ErrorResponse($"{handle} hasn't generated a report yet."));
}
