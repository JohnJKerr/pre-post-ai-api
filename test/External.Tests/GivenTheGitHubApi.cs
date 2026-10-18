using System.Net;

namespace External.Tests;

internal class GivenTheGitHubApi
{
    private readonly Dictionary<string, List<Func<HttpResponseMessage>>> _responses = [];

    public GivenTheGitHubApi()
    {
        Serving("/user/repos?per_page=100", "user-repos.json");
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

    public GivenTheGitHubApi WithEmptyRepo(string repo)
    {
        _responses[$"/repos/octo-dev/{repo}/stats/contributors"] = [Status(HttpStatusCode.NoContent)];
        _responses[$"/repos/octo-dev/{repo}/stats/punch_card"] = [Status(HttpStatusCode.NoContent)];
        _responses[$"/repos/octo-dev/{repo}/languages"] = [Json("{}")];
        return this;
    }

    public FakeGitHubApi Build() => new(_responses.ToDictionary(r => r.Key, r => new Queue<Func<HttpResponseMessage>>(r.Value)));

    private void Serving(string pathAndQuery, string fixture) =>
        _responses[pathAndQuery] = [Json(File.ReadAllText($"Fixtures/{fixture}"))];

    private static Func<HttpResponseMessage> Json(string body) => () => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) };

    private static Func<HttpResponseMessage> Status(HttpStatusCode status) => () => new HttpResponseMessage(status);
}
