using System.Net;
using System.Net.Sockets;
using Protocol;
using Protocol.Payloads;

namespace ServerSide;

public class Server
{
    private readonly string _host;
    private readonly int _port;

    private TcpListener listener;
    
    private List<Client> clients = new List<Client>();

    public Server(string host = "127.0.0.19", int port = 8080)
    {
        this._host = host;
        this._port = port;

        listener = new TcpListener(IPAddress.Parse(host), port);
    }
    
    public async Task StartAsync()
    {
        try
        {
            listener.Start();
            await Console.Out.WriteLineAsync($"Server started at {_host}:{_port}");

            while (true)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();

                Client client = new Client(tcpClient);

                clients.Add(client);

                client.On("test", response =>
                {
                    MemoryStream payload = (MemoryStream)response;
                    var ms = MessageDeserializer.Deserialize<TextMessage>(payload);
                    Console.Out.WriteLine(ms.Content);
                });
                // TODO: set events handlers ???

                _ = Task.Run(() => client.Processing());
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"ERROR: {ex.Message}");

        }
    }
}