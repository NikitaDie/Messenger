namespace ProtocolCore.Message;
public enum MessageType
{
    Opened = 0,
    Connected = 1,
    Disconnected = 2, 
    Event = 3,
    Ack = 4,
    Error = 5,
}