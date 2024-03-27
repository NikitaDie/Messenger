using System.Text;

namespace Protocol.Payloads.Core;

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
    public static IPayload GetObj(MemoryStream stream)
    {
        throw new NotImplementedException();
    }

    protected abstract string GetJson();
    
}