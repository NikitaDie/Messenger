using System.Text;
using System.Text.Json;

namespace ProtocolCore.Payloads.Core;

public abstract class JsonPayload : IPayload
{
    public MemoryStream GetStream()
    {
        MemoryStream memStream = new MemoryStream();

        string json = GetJson();

        byte[] bytes = Encoding.UTF8.GetBytes(json);
        memStream.Write(bytes, 0, bytes.Length);

        return memStream;
    }

    public abstract Type GetPayloadType();

    public static T GetObj<T>(Stream stream)
        where T : JsonPayload
    {
        stream.Position = 0;
        string s;
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        { 
            s = reader.ReadToEnd();
        }

        return JsonSerializer.Deserialize<T>(s);
    }

    protected abstract string GetJson();
    
}