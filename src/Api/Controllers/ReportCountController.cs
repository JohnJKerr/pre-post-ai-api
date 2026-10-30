using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/report-count")]
public class ReportCountController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new ReportCountResponse(0));
}
