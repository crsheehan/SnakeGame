using System.Diagnostics;
using WebServer;

class Program
{
    static void Main()
    {
        Console.WriteLine("Starting Snake System...\n");

        // Start web server
        Task.Run(() =>
        {
            WebServerHost.Start(mock: true, port: 8080);
        });

        // 1️⃣ Find solution root
        var baseDir = AppContext.BaseDirectory;

        var solutionRoot = Path.GetFullPath(
            Path.Combine(baseDir, "..", "..", "..", ".."));

        // 2️⃣ Build path to GUI.csproj
        var guiProject = Path.Combine(
            solutionRoot,
            "SnakeClient",
            "GUI",
            "GUI.csproj");

        if (!File.Exists(guiProject))
        {
            Console.WriteLine("ERROR: Could not find GUI.csproj");
            Console.WriteLine(guiProject);
            return;
        }

        // 3️⃣ Start GUI
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{guiProject}\" --urls=http://localhost:5204",
            WorkingDirectory = solutionRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = Process.Start(psi);

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                Console.WriteLine("[GUI] " + e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                Console.WriteLine("[GUI ERROR] " + e.Data);
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        Console.WriteLine("\nSystem running:");
        Console.WriteLine("• Game UI: http://localhost:5204");
        Console.WriteLine("• Web Scores: http://localhost:8080");
        Console.WriteLine("\nPress Ctrl+C to exit.");

        Thread.Sleep(Timeout.Infinite);
    }
}