using Api.Models;
using Domain;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(IProfileRepository profiles) : ControllerBase
{
    [HttpGet("{handle}")]
    public async Task<IActionResult> Get(string handle)
    {
        var profile = await profiles.GetByHandle(handle);
        if (profile is null) return NotFound(new ErrorResponse($"{handle} hasn't generated a report yet."));

        return Ok(ReportResponse.From(Report.From(profile)));
    }
}
