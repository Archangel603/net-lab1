using Shared.Message;

namespace Client.Socket;

public class EventSubManager : IDisposable
{
    private readonly SocketService _socketService;
    private readonly Dictionary<Type, List<EventListener>> _listeners = new();

    public EventSubManager(SocketService socketService)
    {
        this._socketService = socketService;
    }

    public void Subscribe<T>(Func<T, Task> handler)
    {
        var messageType = typeof(T);
        
        if (!this._listeners.ContainsKey(messageType))
            this._listeners[messageType] = new();
        
        var listener = this._socketService.Subscribe(handler);

        this._listeners[messageType].Add(listener);
    }

    public void Dispose()
    {
        foreach (var (messageType, handlers) in this._listeners)
        {
            foreach (var handler in handlers)
            {
                this._socketService.Unsubscribe(messageType, handler);
            }
        }
    }
}