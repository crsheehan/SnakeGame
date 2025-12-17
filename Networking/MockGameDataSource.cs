namespace Networking;

public class MockGameDataSource : IGameDataSource
{
    public MockGameDataSource(string playerName)
    {
        this.playerName = playerName;
        IsConnected = false;
    }

    private string playerName;
    
    public bool IsConnected { get; private set; }

    public event Action<string>? OnMessageReceived;
    public void Start()
    {
        //Set connected to true
        IsConnected = true;
        
        //Start sending information! (This is hard coded)
        try
        {
            while (IsConnected)
            {
                string message = "hi";
                OnMessageReceived?.Invoke(message);
            }
        }
        catch
        {
            Stop();
        }
        
    }

    /// <summary>
    /// Stop connection, set IsConnected to false to stop sending information. 
    /// </summary>
    public void Stop()
    {
        IsConnected = false;
    }

    public void Send(string message)
    {
        ;
    }

    private List<String> jsons = new List<string>();


}