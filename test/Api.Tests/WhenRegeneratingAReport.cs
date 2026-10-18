using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenRegeneratingAReport(ApiFactory factory) : IAsyncLifetime
{
    private ReportResponse _saved = null!;

    public async Task InitializeAsync()
    {
        var client = factory.CreateClientFor(FakeApiUser.SignedInAs("Second-Go"));
        await client.PostAsJsonAsync("/api/reports", new { });
        await client.PostAsJsonAsync("/api/reports", new { cutoff = "2022-11-30" });
        _saved = (await client.GetFromJsonAsync<ReportResponse>("/api/reports/second-go"))!;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void The_earlier_report_is_replaced() => _saved.Cutoff.Label.ShouldBe("ChatGPT launch");
}
