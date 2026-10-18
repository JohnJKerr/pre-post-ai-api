using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGettingASavedReport(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync()
    {
        var profiles = factory.Services.GetRequiredService<IProfileRepository>();
        await profiles.Save(new GivenAProfile().For("octo-dev").Build());
        _response = await factory.CreateClient().GetAsync("/api/reports/Octo-Dev");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_is_found_whatever_the_handle_s_case() => _response.StatusCode.ShouldBe(HttpStatusCode.OK);

    [Fact]
    public async Task It_is_the_saved_user_s_report() =>
        (await _response.Content.ReadFromJsonAsync<ReportResponse>())!.Handle.ShouldBe("octo-dev");

    [Fact]
    public async Task It_is_built_from_the_saved_activity() =>
        (await _response.Content.ReadFromJsonAsync<ReportResponse>())!.Score.Total.ShouldBe(78);
}
