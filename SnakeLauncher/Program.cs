using WebServer;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.WriteLine(" Starting Snake System...\n");

        // Start web server
        Task.Run(() =>
        {
            WebServerHost.Start(mock: true, port: 8080);
        });

        // Start GUI client
        Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = @"run --project ..\SnakeClient\GUI",
            UseShellExecute = true
        });


        Console.WriteLine("\n System running:");
        Console.WriteLine("• Game UI: http://localhost:5204");
        Console.WriteLine("• Web Scores: http://localhost:8080");
        Console.WriteLine("\nPress Ctrl+C to exit.");

        Thread.Sleep(Timeout.Infinite);
    }
}