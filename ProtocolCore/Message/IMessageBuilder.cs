namespace ProtocolCore.Message;

public interface IMessageBuilder
{
    MemoryStream GetStream(ProtoMessage pm);
    ProtoMessage GetProtoMessage(MemoryStream memStream);
}