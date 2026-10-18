using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Api.Authentication;

public static class GitHubSignIn
{
    public const string Scheme = "GitHub";

    // A GitHub App's user authorisation takes no scopes: the token can do only what the
    // app's permissions allow, which is Metadata: read.
    public static IServiceCollection AddGitHubSignIn(this IServiceCollection services, IConfiguration configuration)
    {
        var appSlug = configuration["GitHub:AppSlug"] ?? throw new InvalidOperationException("GitHub:AppSlug isn't set.");
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "pre-post-session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            })
            .AddOAuth(Scheme, options =>
            {
                options.ClientId = configuration["GitHub:ClientId"]!;
                options.ClientSecret = configuration["GitHub:ClientSecret"]!;
                options.CallbackPath = "/api/auth/callback";
                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";
                options.Events.OnCreatingTicket = AddGitHubIdentity;
                options.Events.OnTicketReceived = context => InstallTheAppFirst(context, appSlug);
                options.Events.OnRemoteFailure = ReturnToTheFrontEnd;
            });

        return services;
    }

    // A user token reaches only the accounts the app is installed on. Without an installation the
    // report would be almost empty, so send them to install the app instead of signing them in.
    private static async Task InstallTheAppFirst(TicketReceivedContext context, string appSlug)
    {
        var oauth = (OAuthOptions)context.Options;
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/installations?per_page=1");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.Principal!.FindFirstValue(ApiUser.AccessTokenClaim));
        request.Headers.UserAgent.ParseAdd("pre-post-ai");
        using var response = await oauth.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
        response.EnsureSuccessStatusCode();

        using var installations = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        if (installations.RootElement.GetProperty("total_count").GetInt32() > 0) return;

        context.Response.Redirect($"https://github.com/apps/{appSlug}/installations/new");
        context.HandleResponse();
    }

    private static Task ReturnToTheFrontEnd(RemoteFailureContext context)
    {
        context.Response.Redirect(context.HttpContext.RequestServices.GetRequiredService<FrontEnd>().Origin + "/");
        context.HandleResponse();
        return Task.CompletedTask;
    }

    private static async Task AddGitHubIdentity(OAuthCreatingTicketContext context)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
        request.Headers.UserAgent.ParseAdd("pre-post-ai");
        using var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
        response.EnsureSuccessStatusCode();

        using var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        context.Identity!.AddClaim(new Claim(ClaimTypes.Name, user.RootElement.GetProperty("login").GetString()!));
        context.Identity.AddClaim(new Claim(ApiUser.AccessTokenClaim, context.AccessToken!));
        EndTheSessionWithTheToken(context);
    }

    // Expiring user tokens last 8 hours. The session holds only the access token, so it ends
    // when the token does and never slides past it.
    private static void EndTheSessionWithTheToken(OAuthCreatingTicketContext context)
    {
        if (context.ExpiresIn is not { } lifetime) return;

        context.Properties.ExpiresUtc = context.Options.TimeProvider!.GetUtcNow() + lifetime;
        context.Properties.AllowRefresh = false;
    }
}
