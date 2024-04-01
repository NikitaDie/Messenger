using ProtocolCore.Message;

namespace ProtocolCore;

public interface ITransport : IDisposable
{
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<IMessage> OnReceived;
    public event Action<Exception> OnError;

    void Initialize();
    Task SendAsync(IMessage items);
    Task DisconnectAsync();
}