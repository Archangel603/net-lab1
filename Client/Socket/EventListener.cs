namespace Client.Socket;

public class EventListener
{
    private Delegate _handler;

    public object Handler => this._handler;
    
    public EventListener(Delegate handler)
    {
        this._handler = handler;
    }

    public async Task Execute(object body)
    {
        await (_handler.DynamicInvoke(body) as Task);
    }
}