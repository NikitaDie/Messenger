using System.Net;
using System.Net.Sockets;
using EventTransmitter;
using MessengerProtocolRealization.Message;
using MessengerProtocolRealization.Payloads;
using MessengerProtocolRealization.Transport;

namespace MessengerServer;

public class Server
{
    private readonly string _host;
    private readonly int _port;
    private readonly TcpListener _listener;
    private readonly List<EventDrivenSocket> _clients = new List<EventDrivenSocket>();
    private readonly EventRouter _eventRouter = new EventRouter();

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
                TcpTransport transportClient = new TcpTransport(tcpClient)
                {
                    MessageBuilder = new MessageBuilder(),
                };
                EventDrivenSocket client = new EventDrivenSocket(transportClient);

                transportClient.Initialize();
                
                
                _clients.Add(client);

                // client.On("auth", response =>
                // {
                //     var payload = response[0];
                //     if (payload.Type != typeof(AuthMessage).ToString());
                //         throw new Exception(); //TODO: callback
                //     
                // });
                
                client.On("test", request =>
                {
                    for (int i = 0; i < request.PayloadCount; ++i)
                    {
                        if (request.GetPayloadType(i) == typeof(TextMessage).ToString())
                        {
                            var x = request.GetValue<TextMessage>(i);
                            Console.WriteLine(x.Content);
                        }
                        
                    }
                });
                
                client.On("akaTest", request =>
                {
                    _ = request.CallbackAsync(new TextMessage("Hi, back!"));
                });
                
                //Warning: another Thread!
                HandleClientEvents(client);
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"ERROR: {ex.Message}");

        }
    }
    
    private void HandleClientEvents(EventDrivenSocket client)
    {
        client.OnRawEventAction += (eventName, response) =>
        {
            _eventRouter.RouteEvent(eventName, response);
        };
    }
}