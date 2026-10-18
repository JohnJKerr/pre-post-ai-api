using System.Web;

namespace Api.Tests;

internal static class BrowserExtensions
{
    public static async Task<HttpResponseMessage> SignInWithGitHub(this HttpClient browser, string returnUrl = "/")
    {
        var signIn = await browser.GetAsync($"/api/auth/sign-in?returnUrl={Uri.EscapeDataString(returnUrl)}");
        var state = HttpUtility.ParseQueryString(signIn.Headers.Location!.Query)["state"];
        return await browser.GetAsync($"/api/auth/callback?code=github-code&state={Uri.EscapeDataString(state!)}");
    }
}
