namespace Networking;

/// <summary>
/// Game Data Source
/// Created by Chancellor Sheehan
/// </summary>
public interface IGameDataSource
{
    bool IsConnected { get; }
    
    event Action<string> OnMessageReceived;
    void Start();
    void Stop();

    void Send(string message);
}