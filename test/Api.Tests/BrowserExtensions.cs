using System.Net.Http.Json;
using System.Web;
using Api.Models;

namespace Api.Tests;

internal static class BrowserExtensions
{
    public static Task<HttpResponseMessage> SignInWithGitHub(this HttpClient browser, string? cutoff = null) =>
        browser.AuthoriseOnGitHubFrom(cutoff is null ? "/api/auth/sign-in" : $"/api/auth/sign-in?cutoff={Uri.EscapeDataString(cutoff)}");

    // Follows the API's sign-in redirect to GitHub, which sends the user straight back.
    public static async Task<HttpResponseMessage> AuthoriseOnGitHubFrom(this HttpClient browser, string signInUrl)
    {
        var signIn = await browser.GetAsync(signInUrl);
        var state = HttpUtility.ParseQueryString(signIn.Headers.Location!.Query)["state"];
        return await browser.GetAsync($"/api/auth/callback?code=github-code&state={Uri.EscapeDataString(state!)}");
    }

    // Polls the generation the callback sent the user to watch, as the front end does.
    public static async Task<GenerationResponse> WaitForTheGeneration(this HttpClient browser, HttpResponseMessage callback)
    {
        var id = HttpUtility.ParseQueryString(callback.Headers.Location!.Query)["id"];
        for (var attempt = 0; attempt < 500; attempt++)
        {
            var generation = (await browser.GetFromJsonAsync<GenerationResponse>($"/api/generations/{id}"))!;
            if (generation.Status != "pending") return generation;
            await Task.Delay(20);
        }

        throw new TimeoutException($"Generation {id} is still pending.");
    }
}
