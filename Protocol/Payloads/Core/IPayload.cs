namespace Protocol.Payloads.Core;

public interface IPayload
{
    public MemoryStream GetStream();

    public string GetPayloadType();
}