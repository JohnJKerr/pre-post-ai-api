using Api.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/generations")]
public class GenerationsController(IGenerationRepository generations, TimeProvider time) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var generation = await generations.GetById(id);
        if (generation is null) return NotFound(new ErrorResponse("We couldn't find that report request. Sign in to generate your report."));

        return Ok(GenerationResponse.From(generation.AsOf(time.GetUtcNow())));
    }
}
