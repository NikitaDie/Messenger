using System.Net.Sockets;
using EndpointSocket;
using Protocol;

namespace ServerSide;

public class Client : EventSocket
{
    private NetworkStream netStream = null!;
    
    public Client(TcpClient tcpClient) : base(tcpClient) { }
    
    public void Processing()
    {
        try
        {
            netStream = TcpClient.GetStream();

            ProtoMessageBuilder builder = new ProtoMessageBuilder(netStream);
            
            while (true)
            {
                ProtoMessage protoMessage = builder.Receive();
                base.Emit(protoMessage.Action, protoMessage.PayloadStream);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    
}