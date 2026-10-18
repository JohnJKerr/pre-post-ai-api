using System.Net;
using System.Text.Json.Nodes;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGettingTheSampleReport(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() => _response = await factory.CreateClient().GetAsync("/api/sample-report");

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void The_sample_report_is_returned() => _response.StatusCode.ShouldBe(HttpStatusCode.OK);

    [Fact]
    public async Task The_report_matches_the_front_end_contract()
    {
        var expected = JsonNode.Parse(await File.ReadAllTextAsync("Fixtures/sample-report.json"));
        var actual = JsonNode.Parse(await _response.Content.ReadAsStringAsync());

        JsonDifference.Between(expected, actual).ShouldBeNull();
    }
}
