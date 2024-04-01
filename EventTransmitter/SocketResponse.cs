using ProtocolCore.Message;
using ProtocolCore.Payloads.Core;

namespace EventTransmitter;

public class SocketResponse
{
    private readonly IMessage _message;
    private readonly EventDrivenSocket _drivenSocket;

    public SocketResponse(IMessage message, EventDrivenSocket drivenSocket)
    {
        this._message = message;
        this._drivenSocket = drivenSocket;
    }
    
    public int PacketId { get => _message.Id; }

    public int PayloadCount
    {
        get => _message.PayloadCount;
    }

    public T GetValue<T>(int index) where T : IReversable 
        => _message.GetValue<T>(index);

    public string? GetPayloadType(int index)
        => _message.GetPayloadType(index);

    public async Task CallbackAsync(params IPayload[] data) 
        => await this._drivenSocket.ClientAckAsync(this.PacketId, data).ConfigureAwait(false);
}