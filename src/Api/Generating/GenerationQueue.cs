using System.Threading.Channels;

namespace Api.Generating;

public class GenerationQueue
{
    // Bounded so a burst of sign-ins waits for room instead of growing without limit.
    private readonly Channel<GenerationRequest> _requests = Channel.CreateBounded<GenerationRequest>(100);

    public ValueTask Enqueue(GenerationRequest request, CancellationToken cancellationToken) =>
        _requests.Writer.WriteAsync(request, cancellationToken);

    public IAsyncEnumerable<GenerationRequest> Requests(CancellationToken cancellationToken) =>
        _requests.Reader.ReadAllAsync(cancellationToken);
}
