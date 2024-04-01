using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ProtocolCore;
using ProtocolCore.Message;

namespace MessengerProtocolRealization.Transport;

public class TcpTransport : ITransport
{
    private readonly TcpClient _client;
    private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
    private NetworkStream? _netStream;
    private readonly IPEndPoint _remoteEndPoint;
    private Task? _receiveTask;
    
    public event Action? OnConnected;
    public event Action? OnDisconnected;
    public event Action<IMessage>? OnReceived;
    public event Action<Exception>? OnError;
    
    public TcpTransport(TcpClient client, string connString = "") 
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        
        if (!string.IsNullOrEmpty(connString))
            _remoteEndPoint = GetIPEndPoint(connString);
        else
            _remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint ?? throw new InvalidOperationException(); //TODO: Exception
    }
    
    public TcpTransport(string connString)
    {
        _client = new TcpClient();
        _remoteEndPoint = GetIPEndPoint(connString);
    }
    
    private static IPEndPoint GetIPEndPoint(string connString)
    {
        return IPEndPoint.Parse(connString);
    }
    
    public void Initialize()
    {
        try
        {
            if (!_client.Connected)
                _client.Connect(_remoteEndPoint);
            
            _netStream = _client.GetStream();
            _receiveTask = Task.Run(ReceiveLoop, _cancellationSource.Token);
            OnConnected?.Invoke();
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            throw;
        }
    }
    
    public async Task InitializeAsync()
    {
        try
        {
            if (!_client.Connected)
                await _client.ConnectAsync(_remoteEndPoint);
            
            _netStream = _client.GetStream();
            _receiveTask = Task.Run(ReceiveLoop, _cancellationSource.Token);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            throw;
        }
    }
 
    private void ReceiveLoop()
    {
        try
        {
            while (!_cancellationSource.Token.IsCancellationRequested)
            {
                Debug.Assert(_netStream != null, nameof(_netStream) + " != null");
                IMessage message = ProtoMessageBuilder.Receive(_netStream);
                OnReceived?.Invoke(message);
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }
    
    public async Task DisconnectAsync()
    {
        await _cancellationSource.CancelAsync();
        if (_receiveTask != null) await _receiveTask;
        _netStream?.Close();
        _client.Close();
        OnDisconnected?.Invoke();
    }
    
    public async Task SendAsync(IMessage item)
    {
        if (_netStream is null)
            throw new NullReferenceException("Stream is null. Call Initialize first!");
        if (!_netStream.CanWrite)
            throw new InvalidOperationException("Stream is not writable.");
        
        using MemoryStream memStream = item.GetStream();
        memStream.Position = 0;
        await memStream.CopyToAsync(_netStream);
    }
    
    public void Dispose()
    {
        _cancellationSource.Dispose();
        DisconnectAsync().Wait();
    }
}