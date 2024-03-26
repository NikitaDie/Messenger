using System.Net.Sockets;

namespace tcpMessengerProtocol;

public class Server
{
    private string host;
    private int port;

    private TcpListener listener;
    
    private List<Client> clients = new List<Client>();

    public Server(string host = "127.0.0.1", int port = 8080)
    {
        this.host = host;
        this.port = port;

        listener = new TcpListener(IPAddress.Parse(host), port);
    }
}