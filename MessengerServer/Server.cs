using System.Net;
using System.Net.Sockets;
using MessengerPayloads;
using ProtocolCore.Payloads.Core;

namespace MessengerServer;

public class Server
{
    private readonly string _host;
    private readonly int _port;

    private readonly TcpListener _listener;
    
    private readonly List<Client> _clients = new List<Client>();

    public Server(string host = "127.0.0.19", int port = 8080)
    {
        this._host = host;
        this._port = port;

        _listener = new TcpListener(IPAddress.Parse(host), port);
    }
    
    public async Task StartAsync()
    {
        try
        {
            _listener.Start();
            await Console.Out.WriteLineAsync($"Server started at {_host}:{_port}");

            while (true)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();

                Client client = new Client(tcpClient);

                _clients.Add(client);

                client.On("auth", response =>
                {
                    var payload = response[0];
                    if (payload.Type != typeof(AuthMessage))
                        throw new Exception(); //TODO: callback
                    
                });
                
                client.On("test", response =>
                {
                    foreach (var payload in response)
                    {
                        if (payload.Type == typeof(TextMessage))
                        {
                            var x = JsonPayload.GetObj<TextMessage>(payload.Stream);
                            Console.WriteLine(x.Content);
                        }

                       
                    }
                    
                });
                // TODO: set events handlers ???
                //Warning: another Thread!
                _ = Task.Run(() => client.Processing());
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"ERROR: {ex.Message}");

        }
    }
}