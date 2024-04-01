using System.Text;
using System.Text.Json;
using ProtocolCore.Payloads.Core;

namespace MessengerProtocolRealization.Payloads;

public class AuthMessage : JsonPayload
{
    public string Login { get; set; }
    public string Password { get; set; }

    public AuthMessage(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public override Type GetPayloadType()
        => GetType();
    
    public new static AuthMessage GetObj(MemoryStream stream)
    {
        stream.Position = 0;
        string s;
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        { 
            s = reader.ReadToEnd();
        }

        return JsonSerializer.Deserialize<AuthMessage>(s);
    }
}