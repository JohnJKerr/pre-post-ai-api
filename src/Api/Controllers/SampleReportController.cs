using Api.Models;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/sample-report")]
public class SampleReportController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(ReportResponse.From(Report.From(SampleProfile.Generate())));
}
