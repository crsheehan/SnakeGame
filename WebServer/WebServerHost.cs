using Networking;

namespace WebServer;

public static class WebServerHost
{
    public static void Start(bool mock, int port)
    {
        Console.WriteLine($"[WebServer] Listening on http://localhost:{port}");

        if (mock)
            Server.StartServer(WebServer.MockHandleHttpConnection, port);
        else
            Server.StartServer(WebServer.HandleHttpConnection, port);
    }
}