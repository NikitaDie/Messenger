using ProtocolCore.Payloads.Core;

namespace MessengerProtocolRealization.Payloads;

public class TextMessage : JsonPayload
{
    // public string Author { get; set; } 
    // public DateTime CreationDateTime { get; set; }
    public string Content { get; set; }

    public TextMessage(string content)
    {
        Content = content;
    }
    
    public override Type GetPayloadType()
        => GetType();
}