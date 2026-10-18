namespace External.Tests;

internal class GivenTheGitHubApi
{
    private readonly FakeGitHubApi _api = new FakeGitHubApi()
        .Serving("/user/repos?per_page=100", "user-repos.json")
        .Serving("/repos/octo-dev/ledger-sync/stats/contributors", "ledger-sync-contributors.json")
        .Serving("/repos/octo-dev/ledger-sync/stats/punch_card", "ledger-sync-punch-card.json")
        .Serving("/repos/octo-dev/ledger-sync/languages", "ledger-sync-languages.json")
        .Serving("/repos/octo-dev/side-project/stats/contributors", "side-project-contributors.json")
        .Serving("/repos/octo-dev/side-project/stats/punch_card", "side-project-punch-card.json")
        .Serving("/repos/octo-dev/side-project/languages", "side-project-languages.json");

    public FakeGitHubApi Build() => _api;
}
