using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGettingAReportNobodyHasGenerated(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() => _response = await factory.CreateClient().GetAsync("/api/reports/nobody-yet");

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_is_not_found() => _response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

    [Fact]
    public async Task The_message_says_nobody_has_generated_one() =>
        (await _response.Content.ReadFromJsonAsync<ErrorBody>())!.Message.ShouldBe("nobody-yet hasn't generated a report yet.");

    private record ErrorBody(string Message);
}
