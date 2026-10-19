using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Api.Generating;
using Domain;
using Domain.GitHub;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Api.Authentication;

// Signing in doesn't start a session. The callback hands the user's token to a background job that
// generates their report, and sends them to the front end to watch it. Nothing keeps the token.
public static class GitHubSignIn
{
    public const string Scheme = "GitHub";
    public const string CutoffItem = "cutoff";

    private const string GitHubIdClaim = "github_id";
    private const string AccessTokenClaim = "github_access_token";

    // A GitHub App's user authorisation takes no scopes: the token can do only what the
    // app's permissions allow, which is Metadata: read.
    public static IServiceCollection AddGitHubSignIn(this IServiceCollection services, IConfiguration configuration)
    {
        var appSlug = configuration["GitHub:AppSlug"] ?? throw new InvalidOperationException("GitHub:AppSlug isn't set.");
        // The OAuth handler needs a separate scheme to sign in to, or it signs in to itself and recurses.
        // StartGenerating handles every successful callback, so this cookie is never issued.
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie()
            .AddOAuth(Scheme, options =>
            {
                options.ClientId = configuration["GitHub:ClientId"]!;
                options.ClientSecret = configuration["GitHub:ClientSecret"]!;
                options.CallbackPath = "/api/auth/callback";
                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";
                options.Events.OnCreatingTicket = AddGitHubIdentity;
                options.Events.OnTicketReceived = context => StartGenerating(context, appSlug);
                options.Events.OnRemoteFailure = RecoverFromAFailedSignIn;
            });

        return services;
    }

    private static async Task AddGitHubIdentity(OAuthCreatingTicketContext context)
    {
        using var user = await GetFromGitHub(context.Backchannel, context.Options.UserInformationEndpoint, context.AccessToken!, context.HttpContext.RequestAborted);
        context.Identity!.AddClaim(new Claim(GitHubIdClaim, user.RootElement.GetProperty("id").GetInt64().ToString(CultureInfo.InvariantCulture)));
        context.Identity.AddClaim(new Claim(ClaimTypes.Name, user.RootElement.GetProperty("login").GetString()!));
        context.Identity.AddClaim(new Claim(AccessTokenClaim, context.AccessToken!));
    }

    private static async Task StartGenerating(TicketReceivedContext context, string appSlug)
    {
        context.HandleResponse();
        var principal = context.Principal!;
        var user = new GitHubUser(
            long.Parse(principal.FindFirstValue(GitHubIdClaim)!, CultureInfo.InvariantCulture),
            principal.FindFirstValue(ClaimTypes.Name)!,
            principal.FindFirstValue(AccessTokenClaim)!);

        // A user token reaches only the accounts the app is installed on. Without an installation the
        // report would be almost empty, so send them to install the app instead.
        if (!await HasInstalledTheApp(context, user))
        {
            context.Response.Redirect($"https://github.com/apps/{appSlug}/installations/new");
            return;
        }

        var services = context.HttpContext.RequestServices;
        var cutoff = new Cutoff(DateOnly.ParseExact(context.Properties!.Items[CutoffItem]!, "yyyy-MM-dd", CultureInfo.InvariantCulture));
        var generation = Generation.Start(user.Id, user.Handle, cutoff, services.GetRequiredService<TimeProvider>().GetUtcNow());
        await services.GetRequiredService<IGenerationRepository>().Save(generation);
        await services.GetRequiredService<GenerationQueue>().Enqueue(new GenerationRequest(generation, user), context.HttpContext.RequestAborted);

        context.Response.Redirect($"{services.GetRequiredService<FrontEnd>().Origin}/generating?id={generation.Id}");
    }

    private static async Task<bool> HasInstalledTheApp(TicketReceivedContext context, GitHubUser user)
    {
        var backchannel = ((OAuthOptions)context.Options).Backchannel;
        using var installations = await GetFromGitHub(backchannel, "https://api.github.com/user/installations?per_page=1", user.AccessToken, context.HttpContext.RequestAborted);
        return installations.RootElement.GetProperty("total_count").GetInt32() > 0;
    }

    private static async Task<JsonDocument> GetFromGitHub(HttpClient backchannel, string url, string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd("pre-post-ai");
        using var response = await backchannel.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
    }

    // An install that starts on GitHub's app page sends the user here without the state this
    // handler created, so the callback fails. They've authorised the app, so signing in again
    // returns straight away.
    private static Task RecoverFromAFailedSignIn(RemoteFailureContext context)
    {
        var installedFromGitHub = context.Request.Query["setup_action"] == "install";
        context.Response.Redirect(installedFromGitHub
            ? "/api/auth/sign-in"
            : context.HttpContext.RequestServices.GetRequiredService<FrontEnd>().Origin + "/?error=sign-in-failed");
        context.HandleResponse();
        return Task.CompletedTask;
    }
}
