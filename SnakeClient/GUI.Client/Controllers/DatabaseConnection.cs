using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace GUI.Client.Controllers;

/// <summary>
/// Provides fucntionality for updating SQL database for Snake
/// Created by Chancellor Sheehan
/// </summary>
public class DatabaseConnection
{
    /// <summary>
    /// The connection string.
    /// Your uID login name serves as both your database name and your uid
    /// </summary>
    private const string ConnectionString = "server=atr.eng.utah.edu;" +
                                            "database=u1455910;" +
                                            "uid=u1455910;" +
                                            "password=Nestedseltzer3!";

    /// <summary>
    /// This method adds a new GameID and StartTime to Games table in Database
    /// </summary>
    /// <returns>Game ID</returns>
    public int AddNewGame(string start)
    {
        using MySqlConnection conn = new MySqlConnection(ConnectionString);

        try
        {
            //Open the connection
            conn.Open();

            //Create command to add new row to the game table
            MySqlCommand newGame = conn.CreateCommand();
            newGame.CommandText = "INSERT INTO Games (StartTime, EndTime) VALUES (@start, NULL);";
            newGame.Parameters.AddWithValue("@start", start); //This binds start to @start!

            //Execute command!
            newGame.ExecuteNonQuery();

            //Now get GameID with sql query
            MySqlCommand getGame = conn.CreateCommand();
            getGame.CommandText = "SELECT LAST_INSERT_ID();";

            using MySqlDataReader reader = getGame.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        throw new Exception("Failed to insert game.");
    }


    /// <summary>
    /// Adds a new player to the Players table. Sets player ID, Player name, max score, startTime. 
    /// </summary>
    public void AddNewPlayer(int playerId, int gameId, string playerName, int maxScore, string startTime)
    {
        using MySqlConnection conn = new MySqlConnection(ConnectionString);
        try
        {
            //Open connection
            conn.Open();

            //Create command to add player
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO Players (ID, GameID, Name, MaxScore, EnterTime, LeaveTime) " +
                "VALUES (@playerID, @gameID, @playerName, @maxScore, @startTime, NULL);";

            // link parameters
            cmd.Parameters.AddWithValue("@playerID", playerId);
            cmd.Parameters.AddWithValue("@gameID", gameId);
            cmd.Parameters.AddWithValue("@playerName", playerName);
            cmd.Parameters.AddWithValue("@maxScore", maxScore);
            cmd.Parameters.AddWithValue("@startTime", startTime);

            // execute command
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in AddNewPlayer: " + ex.Message);
        }
    }

    /// <summary>
    /// This updates a players max score in the database
    /// </summary>
    /// <param name="snakeId">Player id to update</param>
    /// <param name="gameId">Game id to update</param>
    /// <param name="score">Score to update to</param>
    public void UpdatePlayerScore(int snakeId, int gameId, int score)
    {
        using MySqlConnection conn = new MySqlConnection(ConnectionString);
        try
        {
            //Open connection
            conn.Open();

            //Create command to update player score
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Players SET MaxScore = @score WHERE ID = @playerID AND GameID = @gameID";


            // link parameters
            cmd.Parameters.AddWithValue("@playerID", snakeId);
            cmd.Parameters.AddWithValue("@gameID", gameId);
            cmd.Parameters.AddWithValue("@score", score);

            // execute command
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error with updating player score: " + ex.Message);
        }
    }

    /// <summary>
    /// When the player disconnecs from the game server, this method adds the leavetime to the clients row in Players,
    /// and the game row in Games tables in the database
    /// </summary>
    public void SetPlayerEndTime(int playerId, int gameId, string endTime)
    {
        using MySqlConnection conn = new MySqlConnection(ConnectionString);
        try
        {
            //Open connection
            conn.Open();

            //Create command to update player score
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Players SET LeaveTime = @leave WHERE ID = @playerID AND GameID = @gameID";


            // link parameters
            cmd.Parameters.AddWithValue("@playerID", playerId);
            cmd.Parameters.AddWithValue("@gameID", gameId);
            cmd.Parameters.AddWithValue("@leave", endTime);

            // execute command
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error with updating player leavetime: " + ex.Message);
        }
    }

    /// <summary>
    /// Sets the end time for a specific game on the webserver
    /// </summary>
    /// <param name="gameId">Game to update endtime</param>
    /// <param name="endTime">time</param>
    public void SetGameEndTime(int gameId, string endTime)
    {
        using MySqlConnection conn = new MySqlConnection(ConnectionString);
        try
        {
            //Open connection
            conn.Open();

            //Create command to update game endtime
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Games SET EndTime = @leave WHERE ID = @gameID";


            // link parameters
            cmd.Parameters.AddWithValue("@gameID", gameId);
            cmd.Parameters.AddWithValue("@leave", endTime);

            // execute command
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error with updating game leavetime: " + ex.Message);
        }
    }

    /// <summary>
    /// Gets a the game table 
    /// </summary>
    /// <returns>A list of Games row objects to use for displaying games table in html</returns>
    public List<GamesRow> GetGamesTable()
    {
        using MySqlConnection conn = new MySqlConnection(ConnectionString);
        try
        {
            //Open connection
            conn.Open();

            //Create command to get games table
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Games";
            List<GamesRow> games = new List<GamesRow>();
            
            

            // Execute the command and cycle through the DataReader object
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                GamesRow row = new GamesRow
                {
                    gameID = (int)reader["ID"],
                    start = reader["StartTime"].ToString(),
                    end = reader["EndTime"].ToString()
                };


                Console.WriteLine($"Game ID: {row.gameID},  StartTime: {row.start}, EndTime: {row.end}");
                games.Add(row);
            }


            return games;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error wgetting Games table " + ex.Message);
        }

        return new List<GamesRow>();
    }

    /// <summary>
    /// Returns a list of playersRow object, which represent a row in the players database
    /// </summary>
    /// <param name="GameID">Game ID</param>
    /// <returns>List of Players rows that correspond to the game ID</returns>
    public List<PlayersRow> GetPlayersTable(int GameID)
    {
        using MySqlConnection conn = new MySqlConnection(ConnectionString);
        try
        {
            //Open connection
            conn.Open();

            //Create command to get games table
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Players WHERE GameID = @GameID";
        

            //Link parameter
            cmd.Parameters.AddWithValue("@gameID", GameID);

            //Create players list to return
            List<PlayersRow> players = new List<PlayersRow>();
            
            // Execute the command and cycle through the DataReader object
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PlayersRow row = new PlayersRow();


                row.Id = (int)reader["ID"];
                row.gameID = (int)reader["GameID"];
                row.name = reader["Name"].ToString();
                row.maxScore = (int)reader["MaxScore"];
                row.start = reader["EnterTime"].ToString();
                row.end = reader["LeaveTime"].ToString();

                Console.WriteLine($"Game ID: {row.gameID},  StartTime: {row.start}, EndTime: {row.end}");
                players.Add(row);
            }


            return players;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error getting Games table " + ex.Message);
        }

        return new List<PlayersRow>();
    }

    /// <summary>
    /// Object representing a row in the Games table in SQL
    /// </summary>
    public class GamesRow
    {
        /// <summary>
        /// Unique game ID
        /// </summary>
        public int gameID { get; set; }
        
        /// <summary>
        /// Start time of Game
        /// </summary>
        public string? start { get; set; }
        
        /// <summary>
        /// End time of game
        /// </summary>
        public string? end { get; set; }
    }

    /// <summary>
    /// Object representing a row in the Players table in SQL
    /// </summary>
    public class PlayersRow
    {
        /// <summary>
        /// Unique player ID
        /// </summary>
        public int Id { get; set; } 
        
        /// <summary>
        /// Unique game ID player was seen in
        /// </summary>
        public int  gameID { get; set; }

        /// <summary>
        /// Player name
        /// </summary>
        public string? name { get; set; }
        
        /// <summary>
        /// Max score of player
        /// </summary>
        public int maxScore { get; set; }
        
        /// <summary>
        /// Time player joined the game
        /// </summary>
        public string? start { get; set; }
        
        /// <summary>
        /// Time player left the game
        /// </summary>
        public  string? end { get; set; }
        
    }
}