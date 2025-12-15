using GUI.Client.Controllers;
using Networking;


namespace WebServer;

/// <summary>
/// A Webserver for the Snake game displaying every game and the details of that game. 
/// </summary>
public class WebServer
{
    private const string httpBadHeader =
        "HTTP/1.1 404 Not Found\r\nConnection:close\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n";

    private const string httpOkHeader = "HTTP/1.1 200 OK\r\n" +
                                        "Connection: close\r\n" +
                                        "Content-Type: text/html; " +
                                        "charset=UTF-8\r\n\r\n";

    private static DatabaseConnection _dbc = new DatabaseConnection();

    private static bool _mock = true;

    static void Main()
    {
        Console.WriteLine("Starting web server...");
        // PS8: Listens for connections on port 80, runs a delegate when connected

        //Checks to see if server is in mock mode. The database can only be connected to when the local machine is
        //connected to University of Utah wi-fi. Mock is set to true to mimic a database when the local user is NOT
        //connected to University wi-fi. To change, set _mock to false. 
        if (!_mock)
        {
            Server.StartServer(HandleHttpConnection, 80);
        }
        else
        {
            Server.StartServer(MockHandleHttpConnection, 80);
        }

        Console.Read(); // prevent main from returning 
    }

    /// <summary>
    /// When the user is not connected to the University of Utah wifi, this acts as a mock database that just returns numbers with no meaning
    /// </summary>
    /// <param name="client">Client</param>
    private static void MockHandleHttpConnection(NetworkConnection client)
    {
        Console.WriteLine("A client connected");
        string message = client.ReadLine();
        Console.WriteLine(message);

        //This is the home page!
        if (message.Contains("GET / "))
        {
            string response = httpOkHeader;
            response +=
                "<html>\n  <h3>Welcome to the Snake Games Database!</h3>\n  <a href=\"/Games\">View Games</a>\n</html>";
            client.Send(response);
            client.Disconnect();
        }

        //Page for Games table
        else if (message.Contains("GET /Games "))
        {
            string response = httpOkHeader;
            response += "<html>\n" +
                        "  <table border=\"1\">\n" +
                        "    <thead>\n" +
                        "      <tr>\n" +
                        "       <td>ID</td><td>Start</td><td>End</td>\n" +
                        "      </tr>\n" +
                        "    </thead>\n" +
                        "    <tbody>\n";

            for (int r = 1; r < 10; r++)
            {
                response +=
                    $"      <tr>\n" +
                    $"        <td><a href=\"/games?gid={r}\">{r}</a></td>\n" +
                    $"        <td>{r}</td>\n" +
                    $"        <td>{r}</td>\n" +
                    "      </tr>\n";
            }


            response += "    </tbody>\n" +
                        "  </table>\n" +
                        "</html>";

            client.Send(response);
            client.Disconnect();
        }  
        
        else if (message.StartsWith("GET /games?gid="))
        {
            //Create players list
         

            // Get index of game id that the user requests
            int start = message.IndexOf("gid=") + 4;
            int end = message.IndexOf(' ', start);

            string gidString = message.Substring(start, end - start);
            

            //Succesfully got players table to display, now send html back
            string response = httpOkHeader;
            response +=
                $"<html>\n" +
                $"  <h3>Stats for Game {gidString}</h3>\n" +
                "  <table border=\"1\">\n" +
                "    <thead>\n" +
                "      <tr>\n" +
                "        <td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td>\n" +
                "      </tr>\n" +
                "    </thead>\n" +
                "    <tbody>\n";
            
                for (int p = 0; p < 20; p++)
                {
                    response += $"      <tr>\n" +
                                $"        <td>{p}</td><td>{p}</td><td>{p}</td><td>{p}</td><td>{p}</td>\n" +
                                "      </tr>\n";
                }


            response +=
                "    </tbody>\n" +
                "  </table>\n</html>";

            //Send response and disconnect
            client.Send(response);
            client.Disconnect();
        }

        
        //Bad request
        else
        {
            string response = httpBadHeader;
            response += "that is a bad webpage";
            client.Send(response);
            client.Disconnect();
        }
    }

    /// <summary>
    /// Passed into server. Sends html to client to display Snake webserver
    /// </summary>
    /// <param name="client">Client to interact with. </param>
    private static void HandleHttpConnection(NetworkConnection client)
    {
        Console.WriteLine("A client connected");
        string message = client.ReadLine();
        Console.WriteLine(message);

        //This is the home page!
        if (message.Contains("GET / "))
        {
            string response = httpOkHeader;
            response +=
                "<html>\n  <h3>Welcome to the Snake Games Database!</h3>\n  <a href=\"/Games\">View Games</a>\n</html>";
            client.Send(response);
            client.Disconnect();
        }

        //Page for Games table
        else if (message.Contains("GET /Games "))
        {
            List<DatabaseConnection.GamesRow> games = _dbc.GetGamesTable();

            string response = httpOkHeader;
            response += "<html>\n" +
                        "  <table border=\"1\">\n" +
                        "    <thead>\n" +
                        "      <tr>\n" +
                        "       <td>ID</td><td>Start</td><td>End</td>\n" +
                        "      </tr>\n" +
                        "    </thead>\n" +
                        "    <tbody>\n";

            //Add reach row
            foreach (DatabaseConnection.GamesRow r in games)
            {
                response +=
                    $"      <tr>\n" +
                    $"        <td><a href=\"/games?gid={r.gameID}\">{r.gameID}</a></td>\n" +
                    $"        <td>{r.start}</td>\n" +
                    $"        <td>{r.end}</td>\n" +
                    "      </tr>\n";
            }

            response += "    </tbody>\n" +
                        "  </table>\n" +
                        "</html>";

            client.Send(response);
            client.Disconnect();
        }

        else if (message.StartsWith("GET /games?gid="))
        {
            //Create players list
            List<DatabaseConnection.PlayersRow> players = null;

            // Get index of game id that the user requests
            int start = message.IndexOf("gid=") + 4;
            int end = message.IndexOf(' ', start);

            string gidString = message.Substring(start, end - start);

            if (int.TryParse(gidString, out int gid))
            {
                players = _dbc.GetPlayersTable(gid);
            }
            else
            {
                client.Send(httpBadHeader + "Invalid game id");
                client.Disconnect();
            }

            //Succesfully got players table to display, now send html back
            string response = httpOkHeader;
            response +=
                $"<html>\n" +
                $"  <h3>Stats for Game {gid}</h3>\n" +
                "  <table border=\"1\">\n" +
                "    <thead>\n" +
                "      <tr>\n" +
                "        <td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td>\n" +
                "      </tr>\n" +
                "    </thead>\n" +
                "    <tbody>\n";

            //loop through players and add html
            if (players != null)
                foreach (DatabaseConnection.PlayersRow p in players)
                {
                    response += $"      <tr>\n" +
                                $"        <td>{p.Id}</td><td>{p.name}</td><td>{p.maxScore}</td><td>{p.start}</td><td>{p.end}</td>\n" +
                                "      </tr>\n";
                }


            response +=
                "    </tbody>\n" +
                "  </table>\n</html>";

            //Send response and disconnect
            client.Send(response);
            client.Disconnect();
        }

        else
        {
            string response = httpBadHeader;
            response += "that is a bad webpage";
            client.Send(response);
            client.Disconnect();
        }
    }
}