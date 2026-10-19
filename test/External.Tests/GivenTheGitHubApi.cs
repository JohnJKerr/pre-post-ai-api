using System.Net;
using System.Text.Json.Nodes;

namespace External.Tests;

internal class GivenTheGitHubApi
{
    private readonly Dictionary<string, List<Func<HttpResponseMessage>>> _responses = [];

    public GivenTheGitHubApi()
    {
        Serving("repos ", "repos.json");
        Serving("calendar 2024", "calendar-2025.json");
        Serving("calendar 2025", "calendar-2026.json");
        Serving("pull requests ", "merged-pull-requests.json");
        foreach (var repo in new[] { "ledger-sync", "side-project" }) Serving($"history octo-dev/{repo} ", $"{repo}-history.json");
    }

    public GivenTheGitHubApi WithTheTokenExpiredBeforeFetching(string repo)
    {
        _responses[$"history octo-dev/{repo} "] = [Status(HttpStatusCode.Unauthorized)];
        return this;
    }

    public GivenTheGitHubApi WithASecondaryRateLimitFetching(string repo)
    {
        _responses[$"history octo-dev/{repo} "].Insert(0, () =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.Zero);
            return response;
        });
        return this;
    }

    public GivenTheGitHubApi WithAnErrorFetching(string repo)
    {
        _responses[$"history octo-dev/{repo} "] = [Json("""{ "data": null, "errors": [{ "message": "Something went wrong." }] }""")];
        return this;
    }

    public GivenTheGitHubApi WithEmptyRepo(string repo)
    {
        _responses[$"history octo-dev/{repo} "] = [Json("""{ "data": { "repository": { "defaultBranchRef": null } } }""")];
        return this;
    }

    public GivenTheGitHubApi WithASecondPageOfRepos()
    {
        var firstPage = Fixture("repos.json");
        firstPage["data"]!["viewer"]!["repositories"]!["pageInfo"]!["hasNextPage"] = true;
        _responses["repos "] = [Json(firstPage.ToJsonString())];
        Serving("repos repos-1", "repos-page-2.json");
        return WithEmptyRepo("old-tool");
    }

    public GivenTheGitHubApi WithASecondPageOfCommits(string repo)
    {
        var firstPage = Fixture($"{repo}-history.json");
        var history = firstPage["data"]!["repository"]!["defaultBranchRef"]!["target"]!;
        history["mine"]!["pageInfo"]!["hasNextPage"] = true;
        var secondPage = firstPage.DeepClone();
        secondPage["data"]!["repository"]!["defaultBranchRef"]!["target"]!["mine"] = JsonNode.Parse("""
            {
              "pageInfo": { "hasNextPage": false, "endCursor": "page-2" },
              "nodes": [{ "author": { "date": "2025-06-12T09:00:00+01:00" }, "additions": 8, "deletions": 1 }]
            }
            """);
        _responses[$"history octo-dev/{repo} "] = [Json(firstPage.ToJsonString())];
        _responses[$"history octo-dev/{repo} {repo}-1"] = [Json(secondPage.ToJsonString())];
        return this;
    }

    // Repos come most recently pushed first, so one pushed before the pre window ends the list
    // that matters, even with more pages to come.
    public GivenTheGitHubApi WithARepoLastPushedBeforeBothWindows(string repo)
    {
        var page = Fixture("repos.json");
        var repositories = page["data"]!["viewer"]!["repositories"]!;
        repositories["pageInfo"]!["hasNextPage"] = true;
        repositories["nodes"]!.AsArray().Add(JsonNode.Parse($$"""
            {
              "owner": { "login": "octo-dev" }, "name": "{{repo}}", "isPrivate": false,
              "createdAt": "2018-05-01T00:00:00Z", "pushedAt": "2024-12-28T23:59:59Z", "languages": { "edges": [] }
            }
            """));
        _responses["repos "] = [Json(page.ToJsonString())];
        return this;
    }

    public GivenTheGitHubApi WithASecondPageOfMergedPullRequests()
    {
        var firstPage = Fixture("merged-pull-requests.json");
        firstPage["data"]!["search"]!["pageInfo"]!["hasNextPage"] = true;
        _responses["pull requests "] = [Json(firstPage.ToJsonString())];
        _responses["pull requests pull-requests-1"] = [Json("""
            { "data": { "search": { "pageInfo": { "hasNextPage": false, "endCursor": "pull-requests-2" }, "nodes": [{ "mergedAt": "2026-07-01T10:00:00Z" }] } } }
            """)];
        return this;
    }

    public FakeGitHubApi Build() => new(_responses.ToDictionary(r => r.Key, r => new Queue<Func<HttpResponseMessage>>(r.Value)));

    private void Serving(string key, string fixture) => _responses[key] = [Json(File.ReadAllText($"Fixtures/{fixture}"))];

    private static JsonNode Fixture(string name) => JsonNode.Parse(File.ReadAllText($"Fixtures/{name}"))!;

    private static Func<HttpResponseMessage> Json(string body) => () => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) };

    private static Func<HttpResponseMessage> Status(HttpStatusCode status) => () => new HttpResponseMessage(status);
}
