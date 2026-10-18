using System.Globalization;
using Api.Models;
using Domain;
using Domain.GitHub;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(IProfileRepository profiles, IApiUser apiUser, IGitHubMetadata gitHub, TimeProvider time) : ControllerBase
{
    [HttpPost]
    [FromTheFrontEnd]
    public async Task<IActionResult> Generate(GenerateReportRequest request)
    {
        if (apiUser.User is not { } user) return Unauthorized(new ErrorResponse("Sign in with GitHub to generate your report."));

        if (CutoffFrom(request) is not { } cutoff) return UnprocessableEntity(new ErrorResponse($"{request.Cutoff} isn't a valid date. Use YYYY-MM-DD."));

        var now = time.GetUtcNow();
        var post = Window.After(cutoff.Date, DateOnly.FromDateTime(now.UtcDateTime));
        if (post.Weeks == 0) return UnprocessableEntity(new ErrorResponse("Pick a cut-off at least one full week ago, so there's a week to compare."));

        GitHubActivity activity;
        try
        {
            activity = await gitHub.Fetch(user, post.Preceding().Start, post.End);
        }
        catch (GitHubStatsPendingException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ErrorResponse("GitHub is still crunching your repository stats. Try again in a minute."));
        }
        catch (GitHubTokenRejectedException)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Unauthorized(new ErrorResponse("Your GitHub sign-in has expired. Sign in again."));
        }

        var profile = Profile.Assemble(user.Id, user.Handle, cutoff, activity, now);
        if (!profile.HasCommits) return NoContent();

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

    private static Cutoff? CutoffFrom(GenerateReportRequest request)
    {
        if (request.Cutoff is null) return Cutoff.Default;

        var isDate = DateOnly.TryParseExact(request.Cutoff, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
        return isDate ? Cutoff.On(date) : null;
    }
}
