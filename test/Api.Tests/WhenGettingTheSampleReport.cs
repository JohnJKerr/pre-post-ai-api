using System.Net;
using Shouldly;

namespace Api.Tests;

public class WhenGettingTheSampleReport(ApiFactory factory) : IClassFixture<ApiFactory>, IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() => _response = await factory.CreateClient().GetAsync("/api/sample-report");

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void The_sample_report_is_returned() => _response.StatusCode.ShouldBe(HttpStatusCode.OK);
}
