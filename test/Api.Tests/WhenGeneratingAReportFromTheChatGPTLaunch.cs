using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportFromTheChatGPTLaunch(ApiFactory factory) : IAsyncLifetime
{
    private ReportResponse _report = null!;

    public async Task InitializeAsync()
    {
        var response = await factory.CreateClientFor(FakeApiUser.SignedInAs("early-adopter")).PostAsJsonAsync("/api/reports", new { cutoff = "2022-11-30" });
        _report = (await response.Content.ReadFromJsonAsync<ReportResponse>())!;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void The_cutoff_is_named_ChatGPT_launch() => _report.Cutoff.ShouldBe(new CutoffResponse("ChatGPT launch", "2022-11-30"));

    [Fact]
    public void Each_side_spans_every_complete_week_since() => _report.Weeks.ShouldBe(203);
}
