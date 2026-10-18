using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenPollingAGeneration(ApiFactory factory)
{
    [Fact]
    public async Task It_succeeds()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().Build());

        // Act
        var response = await factory.CreateClient().GetAsync($"/api/generations/{id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task A_running_generation_is_pending()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().Build());

        // Act
        var generation = await factory.CreateClient().GetFromJsonAsync<GenerationResponse>($"/api/generations/{id}");

        // Assert
        generation!.Status.ShouldBe("pending");
    }

    [Fact]
    public async Task A_finished_generation_is_ready()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().Completed().Build());

        // Act
        var generation = await factory.CreateClient().GetFromJsonAsync<GenerationResponse>($"/api/generations/{id}");

        // Assert
        generation!.Status.ShouldBe("ready");
    }

    [Fact]
    public async Task A_ready_generation_names_whose_report_to_show()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().Completed().Build());

        // Act
        var generation = await factory.CreateClient().GetFromJsonAsync<GenerationResponse>($"/api/generations/{id}");

        // Assert
        generation!.Handle.ShouldBe("polled-dev");
    }

    [Fact]
    public async Task A_failed_generation_is_failed()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().FailedBecause("GitHub is down.").Build());

        // Act
        var generation = await factory.CreateClient().GetFromJsonAsync<GenerationResponse>($"/api/generations/{id}");

        // Assert
        generation!.Status.ShouldBe("failed");
    }

    [Fact]
    public async Task A_failed_generation_says_why()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().FailedBecause("GitHub is down.").Build());

        // Act
        var generation = await factory.CreateClient().GetFromJsonAsync<GenerationResponse>($"/api/generations/{id}");

        // Assert
        generation!.Message.ShouldBe("GitHub is down.");
    }

    [Fact]
    public async Task A_generation_pending_for_over_30_minutes_has_failed()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().StartedMinutesAgo(31).Build());

        // Act
        var generation = await factory.CreateClient().GetFromJsonAsync<GenerationResponse>($"/api/generations/{id}");

        // Assert
        generation!.Status.ShouldBe("failed");
    }

    [Fact]
    public async Task It_says_which_cutoff_it_compares_either_side_of()
    {
        // Arrange
        var id = await factory.Saved(new GivenAGeneration().WithCutoffOn(new DateOnly(2022, 11, 30)).FailedBecause("GitHub is down.").Build());

        // Act
        var generation = await factory.CreateClient().GetFromJsonAsync<GenerationResponse>($"/api/generations/{id}");

        // Assert
        generation!.Cutoff.ShouldBe(new CutoffResponse("ChatGPT launch", "2022-11-30"));
    }

    [Fact]
    public async Task An_unknown_generation_is_not_found()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/generations/0123456789abcdef0123456789abcdef");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
