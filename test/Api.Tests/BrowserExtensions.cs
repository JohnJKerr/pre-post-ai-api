using System.Web;

namespace Api.Tests;

internal static class BrowserExtensions
{
    public static Task<HttpResponseMessage> SignInWithGitHub(this HttpClient browser, string returnUrl = "/") =>
        browser.AuthoriseOnGitHubFrom($"/api/auth/sign-in?returnUrl={Uri.EscapeDataString(returnUrl)}");

    // Follows the API's sign-in redirect to GitHub, which sends the user straight back.
    public static async Task<HttpResponseMessage> AuthoriseOnGitHubFrom(this HttpClient browser, string signInUrl)
    {
        var signIn = await browser.GetAsync(signInUrl);
        var state = HttpUtility.ParseQueryString(signIn.Headers.Location!.Query)["state"];
        return await browser.GetAsync($"/api/auth/callback?code=github-code&state={Uri.EscapeDataString(state!)}");
    }
}
