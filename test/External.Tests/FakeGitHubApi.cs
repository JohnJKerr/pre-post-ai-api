using System.Net;

namespace External.Tests;

internal class FakeGitHubApi : HttpMessageHandler
{
    private readonly Dictionary<string, Queue<HttpResponseMessage>> _responses = [];

    public List<HttpRequestMessage> Requests { get; } = [];

    public FakeGitHubApi Serving(string pathAndQuery, string fixture) =>
        Responding(pathAndQuery, new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(File.ReadAllText($"Fixtures/{fixture}")) });

    public FakeGitHubApi Responding(string pathAndQuery, HttpResponseMessage response)
    {
        if (!_responses.TryGetValue(pathAndQuery, out var queue)) _responses[pathAndQuery] = queue = new Queue<HttpResponseMessage>();
        queue.Enqueue(response);
        return this;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        var pathAndQuery = request.RequestUri!.PathAndQuery;
        if (!_responses.TryGetValue(pathAndQuery, out var queue)) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));

        var response = queue.Count > 1 ? queue.Dequeue() : queue.Peek();
        return Task.FromResult(response);
    }
}
