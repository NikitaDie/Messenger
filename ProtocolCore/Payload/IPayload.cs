namespace ProtocolCore.Payloads.Core;

public interface IPayload
{
    public MemoryStream GetStream();

    public Type GetPayloadType();
}