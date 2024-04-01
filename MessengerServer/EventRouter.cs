using ProtocolCore;
using ProtocolCore.Payloads.Core;
using EventTransmitter;
using MessengerProtocolRealization.Payloads;

namespace MessengerServer;

public class EventRouter
{
    private readonly Dictionary<string, Action<SocketResponse>> _eventHandlers = new();
    
    public EventRouter()
    {
        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        // _eventHandlers["auth"] = AuthHandler;
        _eventHandlers["test2"] = TestHandler;
    }

    public void RouteEvent(string eventName, SocketResponse request)
    {
        if (_eventHandlers.TryGetValue(eventName, out var handler))
        {
            handler(request);
        }
        else
        {
            throw new Exception($"Unhandled event: {eventName}");
        }
    }
    
    private void AuthHandler(SocketResponse request)
    {
        if (request.GetPayloadType(0) == typeof(AuthMessage).ToString())
        {
            var x = request.GetValue<AuthMessage>(0);
            
        }
        //request.CallbackAsync() TODO: response from Server
    }

    private void TestHandler(SocketResponse request)
    {
        for (int i = 0; i < request.PayloadCount; ++i)
        {
            if (request.GetPayloadType(i) == typeof(TextMessage).ToString())
            {
                var x = request.GetValue<TextMessage>(i);
                Console.WriteLine(x.Content);
            }
            
        }
    }
    
}