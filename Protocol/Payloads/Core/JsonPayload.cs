using System.Text;

namespace Protocol.Payloads.Core;

public abstract class JsonPayload : IPayload
{
    public string GetPayloadType()
    {
        return "json";
    }

    protected abstract string GetJson();
    
    public MemoryStream GetStream()
    {
        MemoryStream memStream = new MemoryStream();

        string json = GetJson();

        byte[] bytes = Encoding.UTF8.GetBytes(json);
        memStream.Write(bytes, 0, bytes.Length);

        return memStream;
    }
}