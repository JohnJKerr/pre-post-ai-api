using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportFromAnInvalidDate(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() =>
        _response = await factory.CreateClientFor(FakeApiUser.SignedInAs("typo-dev")).PostAsJsonAsync("/api/reports", new { cutoff = "2025-02-30" });

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_is_unprocessable() => _response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);

    [Fact]
    public async Task The_message_explains_the_date_format() =>
        (await _response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("2025-02-30 isn't a valid date. Use YYYY-MM-DD.");
}
