using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Shouldly;

namespace Api.Tests;

[Collection(ApiCollection.Name)]
public class WhenGeneratingAReportFromThisWeek(ApiFactory factory) : IAsyncLifetime
{
    private HttpResponseMessage _response = null!;

    public async Task InitializeAsync() =>
        _response = await factory.CreateClientFor(FakeApiUser.SignedInAs("eager-dev")).PostAsJsonAsync("/api/reports", new { cutoff = "2026-10-18" });

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void It_is_unprocessable() => _response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);

    [Fact]
    public async Task The_message_says_a_full_week_must_pass() =>
        (await _response.Content.ReadFromJsonAsync<ErrorResponse>())!.Message.ShouldBe("Pick a cut-off at least one full week ago, so there's a week to compare.");
}
