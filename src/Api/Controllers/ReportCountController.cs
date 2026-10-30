using Api.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/report-count")]
public class ReportCountController(IProfileRepository profiles) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(new ReportCountResponse(await profiles.Count()));
}
