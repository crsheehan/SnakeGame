namespace Networking;

public interface IGameDataSource
{
    bool IsConnected { get; }
    
    event Action<string> OnMessageReceived;
    void Start();
    void Stop();

    void Send(string message);
}