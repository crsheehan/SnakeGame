namespace Networking;

public interface IGameDataSource
{
    bool isConnected { get; }
    
    event Action<string> OnMessageReceived;
    void Start();
    void Stop();
}