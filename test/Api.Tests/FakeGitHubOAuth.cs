using System.Net;

namespace Api.Tests;

internal class FakeGitHubOAuth(string login = "octo-dev", long id = 42, int installations = 1) : HttpMessageHandler
{
    public static FakeGitHubOAuth SigningInAs(string login, long id) => new(login, id);

    public static FakeGitHubOAuth WithoutTheAppInstalled(string login) => new(login, installations: 0);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.RequestUri!.ToString() switch
        {
            "https://github.com/login/oauth/access_token" => """{ "access_token": "user-token", "expires_in": 28800, "refresh_token": "refresh-token", "refresh_token_expires_in": 15811200, "token_type": "bearer", "scope": "" }""",
            "https://api.github.com/user" => $$"""{ "login": "{{login}}", "id": {{id}} }""",
            "https://api.github.com/user/installations?per_page=1" => $$"""{ "total_count": {{installations}}, "installations": [] }""",
            _ => null,
        };

        return Task.FromResult(body is null
            ? new HttpResponseMessage(HttpStatusCode.NotFound)
            : new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json") });
    }
}
