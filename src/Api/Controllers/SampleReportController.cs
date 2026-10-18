using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/sample-report")]
public class SampleReportController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(Report.From(SampleProfile.Generate()));
}
