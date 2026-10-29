using System.Net;
using System.Text.Json.Nodes;

namespace External.Tests;

internal class GivenTheGitHubApi
{
    private const string RepoList = "/user/repos?per_page=100&sort=pushed&direction=desc";

    private readonly Dictionary<string, List<Func<HttpResponseMessage>>> _responses = [];

    public GivenTheGitHubApi()
    {
        Serving(RepoList, "user-repos.json");
        _responses["/graphql"] = [Json(BothCalendars())];
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
        var secondPage = RepoList + "&page=2";
        var firstPage = File.ReadAllText("Fixtures/user-repos.json");
        _responses[RepoList] = [() =>
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
        repos.Add(OlderRepo(repo));
        _responses[RepoList] = [Json(repos.ToJsonString())];
        return this;
    }

    // The next page exists, but this page already ends with a repo pushed before the pre window.
    public GivenTheGitHubApi WithAPageThatEndsBeforeThePreWindow()
    {
        var secondPage = RepoList + "&page=2";
        var repos = JsonNode.Parse(File.ReadAllText("Fixtures/user-repos.json"))!.AsArray();
        repos.Add(OlderRepo("abandoned"));
        _responses[RepoList] = [() =>
        {
            var response = Json(repos.ToJsonString())();
            response.Headers.Add("Link", $"<https://api.github.com{secondPage}>; rel=\"next\"");
            return response;
        }];
        Serving(secondPage, "user-repos-page-2.json");
        return WithEmptyRepo("old-tool");
    }

    private static JsonNode OlderRepo(string repo) => JsonNode.Parse($$"""
        { "id": 9, "name": "{{repo}}", "full_name": "octo-dev/{{repo}}", "private": false, "owner": { "login": "octo-dev" },
          "created_at": "2018-05-01T00:00:00Z", "pushed_at": "2024-12-28T23:59:59Z" }
        """)!;

    public FakeGitHubApi Build() => new(_responses.ToDictionary(r => r.Key, r => new Queue<Func<HttpResponseMessage>>(r.Value)));

    private void Serving(string pathAndQuery, string fixture) =>
        _responses[pathAndQuery] = [Json(File.ReadAllText($"Fixtures/{fixture}"))];

    private static string BothCalendars()
    {
        var pre = JsonNode.Parse(File.ReadAllText("Fixtures/calendar-2025.json"))!;
        var post = JsonNode.Parse(File.ReadAllText("Fixtures/calendar-2026.json"))!;
        return new JsonObject
        {
            ["data"] = new JsonObject
            {
                ["viewer"] = new JsonObject
                {
                    ["w0"] = JsonNode.Parse(pre["data"]!["viewer"]!["contributionsCollection"]!.ToJsonString()),
                    ["w1"] = JsonNode.Parse(post["data"]!["viewer"]!["contributionsCollection"]!.ToJsonString()),
                },
            },
        }.ToJsonString();
    }

    private static Func<HttpResponseMessage> Json(string body) => () => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) };

    private static Func<HttpResponseMessage> Status(HttpStatusCode status) => () => new HttpResponseMessage(status);
}
