﻿using System.Text;
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
    
    public override Type GetPayloadType()
        => GetType();
    
    protected override string GetJson()
    {
        return JsonSerializer.Serialize(this);
    }
    
    public new static TextMessage GetObj(MemoryStream stream)
    {
        stream.Position = 0;
        string s;
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        { 
            s = reader.ReadToEnd();
        }

        return JsonSerializer.Deserialize<TextMessage>(s);
    }
}