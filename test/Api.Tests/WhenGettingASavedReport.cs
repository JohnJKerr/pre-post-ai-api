using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGettingASavedReport(ApiFactory factory)
{
    [Fact]
    public async Task It_is_found_whatever_the_handle_s_case()
    {
        // Arrange
        await factory.Services.GetRequiredService<IProfileRepository>().Save(new GivenAProfile().For("kept-dev").Build());

        // Act
        var response = await factory.CreateClient().GetAsync("/api/reports/Kept-Dev");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task It_is_the_saved_user_s_report()
    {
        // Arrange
        await factory.Services.GetRequiredService<IProfileRepository>().Save(new GivenAProfile().For("kept-dev").Build());

        // Act
        var report = await factory.CreateClient().GetFromJsonAsync<ReportResponse>("/api/reports/kept-dev");

        // Assert
        report!.Handle.ShouldBe("kept-dev");
    }

    [Fact]
    public async Task It_is_built_from_the_saved_activity()
    {
        // Arrange
        await factory.Services.GetRequiredService<IProfileRepository>().Save(new GivenAProfile().For("kept-dev").Build());

        // Act
        var report = await factory.CreateClient().GetFromJsonAsync<ReportResponse>("/api/reports/kept-dev");

        // Assert
        report!.Score.Total.ShouldBe(78);
    }
}
