using ProtocolCore;

namespace EndpointSocket;

public class ListenerImpl : IListener, IComparable<IListener>
{
    private static int _idCounter;
    private readonly int _id;
    private readonly Action? _fn1;
    private readonly Action<PayloadInfo[]>? _fn;
    
    public ListenerImpl(Action<PayloadInfo[]> fn)
    {
        this._fn = fn;
        this._id = ListenerImpl._idCounter++;
    }

    public ListenerImpl(Action? fn)
    {
        this._fn1 = fn;
        this._id = ListenerImpl._idCounter++;
    }
    
    // public void Call(params object[] args)
    // {
    //     if (this._fn != null)
    //         this._fn(args.Length != 0 ? args[0] : (object) null);
    //     else
    //         this._fn1();
    // }

    public void Call(PayloadInfo[] arg)
    {
        if (this._fn != null)
            this._fn(arg);
        else if (this._fn1 != null)
            this._fn1();
        else
        {
            throw new Exception();
        }
    }
    
    public int CompareTo(IListener other) => this.GetId().CompareTo(other.GetId());

    public int GetId() => this._id;
}