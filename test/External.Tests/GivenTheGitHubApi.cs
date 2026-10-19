using System.Net;
using System.Text.Json.Nodes;

namespace External.Tests;

internal class GivenTheGitHubApi
{
    private readonly Dictionary<string, List<Func<HttpResponseMessage>>> _responses = [];

    public GivenTheGitHubApi()
    {
        Serving("/user/repos?per_page=100", "user-repos.json");
        _responses["/graphql"] = [Json(File.ReadAllText("Fixtures/calendar-2025.json")), Json(File.ReadAllText("Fixtures/calendar-2026.json"))];
        foreach (var repo in new[] { "ledger-sync", "side-project" })
        {
            Serving($"/repos/octo-dev/{repo}/stats/contributors", $"{repo}-contributors.json");
            Serving($"/repos/octo-dev/{repo}/stats/punch_card", $"{repo}-punch-card.json");
            Serving($"/repos/octo-dev/{repo}/languages", $"{repo}-languages.json");
        }
    }

    public GivenTheGitHubApi WithStatsStillComputing(string repo, int times)
    {
        var path = $"/repos/octo-dev/{repo}/stats/contributors";
        _responses[path].InsertRange(0, Enumerable.Repeat(Status(HttpStatusCode.Accepted), times));
        return this;
    }

    public GivenTheGitHubApi WithStatsNeverReady(string repo)
    {
        _responses[$"/repos/octo-dev/{repo}/stats/contributors"] = [Status(HttpStatusCode.Accepted)];
        return this;
    }

    public GivenTheGitHubApi WithTheTokenExpiredBeforeFetching(string repo)
    {
        _responses[$"/repos/octo-dev/{repo}/stats/contributors"] = [Status(HttpStatusCode.Unauthorized)];
        return this;
    }

    public GivenTheGitHubApi WithEmptyRepo(string repo)
    {
        _responses[$"/repos/octo-dev/{repo}/stats/contributors"] = [Status(HttpStatusCode.NoContent)];
        _responses[$"/repos/octo-dev/{repo}/stats/punch_card"] = [Status(HttpStatusCode.NoContent)];
        _responses[$"/repos/octo-dev/{repo}/languages"] = [Json("{}")];
        return this;
    }

    public GivenTheGitHubApi WithASecondPageOfRepos()
    {
        const string secondPage = "/user/repos?per_page=100&page=2";
        var firstPage = File.ReadAllText("Fixtures/user-repos.json");
        _responses["/user/repos?per_page=100"] = [() =>
        {
            var response = Json(firstPage)();
            response.Headers.Add("Link", $"<https://api.github.com{secondPage}>; rel=\"next\", <https://api.github.com{secondPage}>; rel=\"last\"");
            return response;
        }];
        Serving(secondPage, "user-repos-page-2.json");
        return WithEmptyRepo("old-tool");
    }

    public GivenTheGitHubApi WithARepoLastPushedBeforeBothWindows(string repo)
    {
        var repos = JsonNode.Parse(File.ReadAllText("Fixtures/user-repos.json"))!.AsArray();
        repos.Add(JsonNode.Parse($$"""
            { "id": 9, "name": "{{repo}}", "full_name": "octo-dev/{{repo}}", "private": false, "owner": { "login": "octo-dev" },
              "created_at": "2018-05-01T00:00:00Z", "pushed_at": "2024-12-28T23:59:59Z" }
            """));
        _responses["/user/repos?per_page=100"] = [Json(repos.ToJsonString())];
        return this;
    }

    public FakeGitHubApi Build() => new(_responses.ToDictionary(r => r.Key, r => new Queue<Func<HttpResponseMessage>>(r.Value)));

    private void Serving(string pathAndQuery, string fixture) =>
        _responses[pathAndQuery] = [Json(File.ReadAllText($"Fixtures/{fixture}"))];

    private static Func<HttpResponseMessage> Json(string body) => () => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) };

    private static Func<HttpResponseMessage> Status(HttpStatusCode status) => () => new HttpResponseMessage(status);
}
