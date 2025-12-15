namespace Networking;

public class TcpGameDataSource : IGameDataSource
{
    private readonly NetworkConnection _network;
    private CancellationTokenSource? _cts;
    private string _server;
    private int _port;
    private string _playerName;

    public bool isConnected => _network.IsConnected;
    public event Action<string>? OnMessageReceived;

    public TcpGameDataSource(NetworkConnection network, string server, int port, string playerName)
    {
        this._network = network;
        this._server = server;
        this._port = port;
        this._playerName = playerName;
    }

    /// <summary>
    /// Starts a network connection, sends player names, and listens for game data! 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Start()
    {
        try
        {
            //Connect
            _network.Connect(_server, _port);

            //Send player name (Snake protocol)
            _network.Send(_playerName + "\n");

            _cts = new CancellationTokenSource();
            Task.Run(() => ReceiveLoop(_cts.Token));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    /// <summary>
    /// Stop listening 
    /// </summary>
    public void Stop()
    {
        _cts?.Cancel();
        _network.Disconnect();
        _network.Dispose();
    }

    private void ReceiveLoop(CancellationToken token)
    {
        try
        {
            while (_network.IsConnected && !token.IsCancellationRequested)
            {
                string message = _network.ReadLine();
                OnMessageReceived?.Invoke(message);
            }
        }
        catch
        {
            Stop();
        }
    }
}