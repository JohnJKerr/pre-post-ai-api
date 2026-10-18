using Api.Models;
using Domain;
using Domain.GitHub;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(IProfileRepository profiles, IApiUser apiUser, IGitHubMetadata gitHub, TimeProvider time) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Generate(GenerateReportRequest request)
    {
        var user = apiUser.User!;
        var now = time.GetUtcNow();
        var cutoff = Cutoff.Default;
        var post = Window.After(cutoff.Date, DateOnly.FromDateTime(now.UtcDateTime));

        var activity = await gitHub.Fetch(user, post.Preceding().Start, post.End);
        var profile = Profile.Assemble(user.Handle, cutoff, activity, now);
        await profiles.Save(profile);

        return Created($"/api/reports/{profile.Handle}", ReportResponse.From(Report.From(profile)));
    }

    [HttpGet("{handle}")]
    public async Task<IActionResult> Get(string handle)
    {
        var profile = await profiles.GetByHandle(handle);
        if (profile is null) return NotFound(new ErrorResponse($"{handle} hasn't generated a report yet."));

        return Ok(ReportResponse.From(Report.From(profile)));
    }
}
