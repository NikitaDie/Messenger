using System.Text.Json;
using ProtocolCore.Payloads.Core;

namespace MessengerPayloads;

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
    
    protected override string GetJson()
    {
        return JsonSerializer.Serialize(this);
    }
}