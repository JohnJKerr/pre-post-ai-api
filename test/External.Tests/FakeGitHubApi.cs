namespace External.Tests;

internal class FakeGitHubApi(Dictionary<string, Queue<Func<HttpResponseMessage>>> responses) : HttpMessageHandler
{
    public List<HttpRequestMessage> Requests { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        var queue = responses[request.RequestUri!.PathAndQuery];
        var respond = queue.Count > 1 ? queue.Dequeue() : queue.Peek();
        return Task.FromResult(respond());
    }
}
