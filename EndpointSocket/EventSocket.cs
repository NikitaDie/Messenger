using System.Net.Sockets;
using Protocol;
using Protocol.Payloads.Core;

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
    
    // public override Emitter Emit(string eventString, params IPayload[] args)
    public Emitter Emit(string eventString, IPayload arg)
    {
        ProtoMessage m = new ProtoMessage()
        {
            Action = eventString,
        };
        //m.SetHeader("testHeader", "hohoho");
        m.SetPayload(arg);
        
        MemoryStream memStream = m.GetStream();
        // Console.Write("Press to send");
        // Console.ReadLine();
        // Console.WriteLine(memStream.Length);
        memStream.CopyTo(_netStream);
        
        return (Emitter) this;
    }
}