using System.Text.Json.Nodes;

namespace External.Tests;

// Every call is a GraphQL query, so responses are keyed by what the query asks for (see GraphQlCall.Key).
internal class FakeGitHubApi(Dictionary<string, Queue<Func<HttpResponseMessage>>> responses) : HttpMessageHandler
{
    private readonly Lock _gate = new();
    private int _inFlight;

    public List<GraphQlCall> Calls { get; } = [];

    public int MostCallsAtOnce { get; private set; }

    // Each answer takes a moment, so calls made at the same time overlap.
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var call = new GraphQlCall(request, JsonNode.Parse(await request.Content!.ReadAsStringAsync(cancellationToken))!);
        Func<HttpResponseMessage> respond;
        lock (_gate)
        {
            Calls.Add(call);
            MostCallsAtOnce = Math.Max(MostCallsAtOnce, ++_inFlight);
            var queue = responses[call.Key];
            respond = queue.Count > 1 ? queue.Dequeue() : queue.Peek();
        }

        await Task.Delay(20, cancellationToken);
        lock (_gate) _inFlight--;
        return respond();
    }
}

internal record GraphQlCall(HttpRequestMessage Request, JsonNode Body)
{
    public string Query => Body["query"]!.GetValue<string>();

    public string? Variable(string name) => Body["variables"]?[name]?.ToString();

    public bool IsForRepos => Query.Contains("repositories(");

    public bool IsForHistory => Query.Contains("history(");

    public bool IsForCalendar => Query.Contains("contributionsCollection(");

    public bool IsForPullRequests => Query.Contains("search(");

    public string Key =>
        IsForRepos ? $"repos {Variable("cursor")}"
        : IsForHistory ? $"history {Variable("owner")}/{Variable("name")} {Variable("cursor")}"
        : IsForCalendar ? $"calendar {Variable("from")![..4]}"
        : IsForPullRequests ? $"pull requests {Variable("cursor")}"
        : throw new InvalidOperationException($"Unexpected query: {Query}");
}
