using System.Net.Sockets;
using ProtocolCore;
using ProtocolCore.Payloads.Core;

namespace EndpointSocket;

public class EventSocket : Emitter
{
    protected readonly TcpClient TcpClient;
    private readonly NetworkStream _netStream;
    
    public EventSocket(TcpClient tcpClient)
    {
        TcpClient = tcpClient;
        _netStream = tcpClient.GetStream();
    }

    public Emitter Emit(string eventString, IEnumerable<IPayload> arg)
    {
        ProtoMessage m = new ProtoMessage()
        {
            Action = eventString,
        };
        m.AddPayload(arg);
        
        using (MemoryStream memStream = m.GetStream())
        {
            memStream.CopyTo(_netStream);
        }
        
        return this;
    }
    
    public Emitter Emit(string eventString, params IPayload[] args)
    {
        return Emit(eventString, new List<IPayload>(args));
    }
    
}