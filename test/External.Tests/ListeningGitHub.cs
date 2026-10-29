using System.Net;
using System.Net.Sockets;

namespace External.Tests;

// Serves the fake API on a real socket so the production handler can decompress responses.
internal sealed class ListeningGitHub : IAsyncDisposable
{
    private readonly HttpListener _listener = new();
    private readonly CancellationTokenSource _stop = new();
    private readonly HttpMessageInvoker _api;
    private readonly List<HttpRequestMessage> _requests = [];
    private readonly Task _serving;

    public ListeningGitHub(FakeGitHubApi api)
    {
        var probe = new TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        var port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();

        _api = new HttpMessageInvoker(api, disposeHandler: false);
        BaseAddress = new Uri($"http://127.0.0.1:{port}");
        _listener.Prefixes.Add(BaseAddress + "/");
        _listener.Start();
        _serving = Serve();
    }

    public Uri BaseAddress { get; }

    public IReadOnlyList<HttpRequestMessage> Requests
    {
        get { lock (_requests) return _requests.ToArray(); }
    }

    private async Task Serve()
    {
        while (!_stop.IsCancellationRequested)
        {
            HttpListenerContext context;
            try { context = await _listener.GetContextAsync(); }
            catch (HttpListenerException) { break; }
            catch (ObjectDisposedException) { break; }

            try { await Respond(context); }
            catch (OperationCanceledException) { break; }
            catch (Exception)
            {
                try { context.Response.Abort(); } catch (HttpListenerException) { }
                if (_stop.IsCancellationRequested) break;
                throw;
            }
        }
    }

    private async Task Respond(HttpListenerContext context)
    {
        var target = context.Request.Url!.PathAndQuery;
        var recorded = new HttpRequestMessage(new HttpMethod(context.Request.HttpMethod), target);
        foreach (string? name in context.Request.Headers)
            if (name is not null) recorded.Headers.TryAddWithoutValidation(name, context.Request.Headers[name]);
        lock (_requests) _requests.Add(recorded);

        using var inbound = new HttpRequestMessage(new HttpMethod(context.Request.HttpMethod), new Uri(BaseAddress, target));
        using var response = await _api.SendAsync(inbound, _stop.Token);
        var body = response.Content is null ? [] : await response.Content.ReadAsByteArrayAsync(_stop.Token);

        context.Response.StatusCode = (int)response.StatusCode;
        CopyHeaders(response, context.Response);
        context.Response.ContentLength64 = body.Length;
        if (body.Length > 0) await context.Response.OutputStream.WriteAsync(body, _stop.Token);
        context.Response.Close();
    }

    private static void CopyHeaders(HttpResponseMessage response, HttpListenerResponse destination)
    {
        foreach (var header in response.Headers)
            destination.Headers[header.Key] = string.Join(", ", header.Value);
        if (response.Content is null) return;
        foreach (var header in response.Content.Headers)
        {
            if (header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)) continue;
            if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                destination.ContentType = string.Join(", ", header.Value);
                continue;
            }
            destination.Headers[header.Key] = string.Join(", ", header.Value);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _stop.Cancel();
        _listener.Close();
        try { await _serving; } catch (Exception) { /* the listener has stopped */ }
        foreach (var request in _requests) request.Dispose();
        _api.Dispose();
        _stop.Dispose();
    }
}
