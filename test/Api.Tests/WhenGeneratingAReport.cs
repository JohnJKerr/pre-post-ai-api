using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReport(ApiFactory factory) : IAsyncLifetime
{
    private HttpClient _client = null!;
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync()
    {
        _client = factory.CreateClientFor(FakeApiUser.SignedInAs("new-dev"));
        _response = await _client.PostAsJsonAsync("/api/reports", new { });
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_is_created() => _response.StatusCode.ShouldBe(HttpStatusCode.Created);

    [Fact]
    public void It_is_located_at_the_user_s_report() => _response.Headers.Location!.ToString().ShouldBe("/api/reports/new-dev");

    [Fact]
    public async Task It_is_for_the_signed_in_user() =>
        (await _response.Content.ReadFromJsonAsync<ReportResponse>())!.Handle.ShouldBe("new-dev");

    [Fact]
    public async Task It_compares_either_side_of_Claude_Opus_4_5_by_default() =>
        (await _response.Content.ReadFromJsonAsync<ReportResponse>())!.Cutoff.ShouldBe(new CutoffResponse("Claude Opus 4.5", "2025-11-24"));

    [Fact]
    public async Task It_is_built_from_the_user_s_GitHub_activity() =>
        (await _response.Content.ReadFromJsonAsync<ReportResponse>())!.Post.Added.ShouldBe(2_000);

    [Fact]
    public async Task It_is_saved_for_anyone_to_view() =>
        (await _client.GetAsync("/api/reports/new-dev")).StatusCode.ShouldBe(HttpStatusCode.OK);
}
