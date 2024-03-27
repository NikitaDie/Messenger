namespace Protocol.Payloads.Core;

public interface IPayload
{
    public MemoryStream GetStream();

    public Type GetPayloadType();

    public static abstract IPayload GetObj(MemoryStream stream);
}