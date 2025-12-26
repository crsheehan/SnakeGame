using Networking;

namespace GUI.Client.MockData;

/// <summary>
/// Game data source that mocks the server used to host multiple clients. Sends fake information to client mimicking snake gameplay
/// Created by Chancellor Sheehan
/// </summary>
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
        IsConnected = true;

        Task.Run(() =>
        {
            try
            {
                var assembly = typeof(MockGameDataSource).Assembly;
                var resourceName = "GUI.Client.MockData.MockServer.txt";

                while (IsConnected) // 🔁 repeat forever
                {
                    using Stream stream = assembly.GetManifestResourceStream(resourceName)
                                          ?? throw new Exception("MockServer.txt not found as embedded resource");

                    using StreamReader reader = new(stream);

                    while (!reader.EndOfStream && IsConnected)
                    {
                        var line = reader.ReadLine();
                        OnMessageReceived?.Invoke(line);

                        Thread.Sleep(1); // match server tick rate
                    }

                    // Optional pause before restarting
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Mock server error: " + e.Message);
            }
        });
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

   
}