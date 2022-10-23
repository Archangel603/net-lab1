using System.Collections.Concurrent;
using Shared.Message.Events;

namespace Server.Services;

public class EventBus
{
    public ConcurrentQueue<IEvent> Events = new();

    public async Task PublishEvent(IEvent e)
    {
        this.Events.Enqueue(e);
    }

    public async IAsyncEnumerable<IEvent> ListenForEvents(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (this.Events.IsEmpty)
            {
                await Task.Delay(100);
                continue;
            }

            if (this.Events.TryDequeue(out var tuple))
            {
                yield return tuple;
            }
        }
    }
}