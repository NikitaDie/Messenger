using System.Collections.Immutable;
using Protocol.Payloads.Core;

namespace EndpointSocket;

public class Emitter
{
    private ImmutableDictionary<string, ImmutableList<IListener>> _callbacks;
    //private ImmutableDictionary<IListener, IListener> _onceCallbacks;

    public Emitter() => this.Off();
  
    // public virtual Emitter Emit(string eventString, params object[] args)
    // {
    //   if (_callbacks.ContainsKey(eventString))
    //   {
    //     foreach (IListener listener in _callbacks[eventString])
    //       listener.Call(args);
    //   }
    //   return this;
    // }

    public virtual Emitter Emit(string eventString, object arg)
    {
      if (_callbacks.ContainsKey(eventString))
      {
        foreach (IListener listener in _callbacks[eventString])
          listener.Call(arg);
      }
      return this;
    }
    
    public Emitter On(string eventString, IListener fn)
    {
      if (!this._callbacks.ContainsKey(eventString))
        this._callbacks = this._callbacks.Add(eventString, ImmutableList<IListener>.Empty);
      ImmutableList<IListener> immutableList = this._callbacks[eventString].Add(fn);
      this._callbacks = this._callbacks.Remove(eventString).Add(eventString, immutableList);
      return this;
    }

    public Emitter On(string eventString, Action? fn)
    {
      ListenerImpl fn1 = new ListenerImpl(fn);
      return this.On(eventString, (IListener) fn1);
    }

    public Emitter On(string eventString, Action<Object> fn)
    {
      ListenerImpl fn1 = new ListenerImpl(fn);
      return this.On(eventString, (IListener) fn1);
    }

    // public Emitter Once(string eventString, IListener fn)
    // {
    //   OnceListener fn1 = new OnceListener(eventString, fn, this);
    //   this._onceCallbacks = this._onceCallbacks.Add(fn, (IListener) fn1);
    //   this.On(eventString, (IListener) fn1);
    //   return this;
    // }

    // public Emitter Once(string eventString, Action? fn)
    // {
    //   ListenerImpl fn1 = new ListenerImpl(fn);
    //   return this.Once(eventString, (IListener) fn1);
    // }

    public Emitter Off()
    {
      this._callbacks = ImmutableDictionary.Create<string, ImmutableList<IListener>>();
      // this._onceCallbacks = ImmutableDictionary.Create<IListener, IListener>();
      return this;
    }

    // public Emitter Off(string eventString)
    // {
    //   try
    //   {
    //     ImmutableList<IListener> immutableList;
    //     if (!this._callbacks.TryGetValue(eventString, out immutableList))
    //       LogManager.GetLogger(Global.CallerName(nameof (Off), 143, "C:\\EngineIoClientDotNet\\Src\\EngineIoClientDotNet.mono\\ComponentEmitter\\Emitter.cs"));
    //     if (immutableList != null)
    //     {
    //       this._callbacks = this._callbacks.Remove(eventString);
    //       foreach (IListener key in immutableList)
    //         this._onceCallbacks.Remove(key);
    //     }
    //   }
    //   catch (Exception ex)
    //   {
    //     this.Off();
    //   }
    //   return this;
    // }
    //
    // public Emitter Off(string eventString, IListener fn)
    // {
    //   try
    //   {
    //     if (this._callbacks.ContainsKey(eventString))
    //     {
    //       ImmutableList<IListener> callback = this._callbacks[eventString];
    //       IListener listener;
    //       this._onceCallbacks.TryGetValue(fn, out listener);
    //       this._onceCallbacks = this._onceCallbacks.Remove(fn);
    //       if (callback.Count > 0)
    //       {
    //         if (callback.Contains(listener ?? fn))
    //         {
    //           ImmutableList<IListener> immutableList = callback.Remove(listener ?? fn);
    //           this._callbacks = this._callbacks.Remove(eventString);
    //           this._callbacks = this._callbacks.Add(eventString, immutableList);
    //         }
    //       }
    //     }
    //   }
    //   catch (Exception ex)
    //   {
    //     this.Off();
    //   }
    //   return this;
    // }
}