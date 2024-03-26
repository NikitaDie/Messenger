using System.Text.Json;
using Protocol.Payloads.Core;

namespace Protocol.Payloads;

public class TextMessage : JsonPayload
{
    // public string Author { get; set; } 
    // public DateTime CreationDateTime { get; set; }
    public string Content { get; set; }

    public TextMessage(string content)
    {
        Content = content;
    }
    protected override string GetJson()
    {
        return JsonSerializer.Serialize(this);
    }
}