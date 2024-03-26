using Protocol.Payloads.Core;

namespace EndpointSocket;

public interface IListener : IComparable<IListener>
{
    int GetId();

    // void Call(params object[] args);
    void Call(object arg);
}