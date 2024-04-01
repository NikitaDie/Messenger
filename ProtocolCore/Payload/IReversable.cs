namespace ProtocolCore.Payloads.Core;

public interface IReversable
{
    static abstract T GetObj<T>(Stream stream);
}