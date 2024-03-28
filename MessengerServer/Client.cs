using System.Net.Sockets;
using EndpointSocket;
using ProtocolCore;

namespace MessengerServer;

public class Client : EventSocket
{
    private NetworkStream? _netStream;
    
    public Client(TcpClient tcpClient) : base(tcpClient) { }
    
    public void Processing()
    {
        try
        {
            _netStream = TcpClient.GetStream();
            
            while (true)
            {
                ProtoMessage protoMessage = ProtoMessageBuilder.Receive(_netStream);
                this.Emit(protoMessage.Action!, protoMessage.PayloadsInfo.ToArray());
            }
        }
        catch (Exception) //TODO: exception
        {

            throw;
        }
    }
    
}