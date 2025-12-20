using System.Data;
using System.Diagnostics;
using System.Text.Json;
using GUI.Client.MockData;
using GUI.Client.Models;
using Networking;


namespace GUI.Client.Controllers;

/// <summary>
/// The network controller is responsible for parsing information received from the network and updating the model
/// based on that information.
/// </summary>
public class NetworkController
{
    /// <summary>
    /// Source of game data!
    /// </summary>
    private IGameDataSource _dataSource = null!;


    /// <summary>
    /// JSON received from the server
    /// </summary>
    private string _messageReceived = string.Empty;


    /// <summary>
    /// World model updated by server
    /// </summary>
    public World _world = null!;

    /// <summary>
    /// Player Id, first message sent from server
    /// </summary>
    public int _playerId;

    /// <summary>
    /// Name of player, used on scoreboard
    /// </summary>
    private string _playerName;

    /// <summary>
    /// bool to check if the world has received a player ID from the server
    /// </summary>
    private bool _havePlayerId = false;

    /// <summary>
    /// bool to check if the world has received a world size from the server
    /// </summary>
    private bool _haveWorldSize = false;

    /// <summary>
    /// bool to check if the world has received all walls from the server
    /// </summary>
    private bool _haveAllWalls = false;

    /// <summary>
    /// Players snake
    /// </summary>
    public Snake MySnake { get; set; }

    /// <summary>
    /// Bool to check if the client is connected to server. Represented by network's connection
    /// </summary>
    public bool IsConnected => _dataSource?.IsConnected ?? false;

    /// <summary>
    /// Maps a snake's ID to it's max score
    /// </summary>
    public Dictionary<int, int> _maxScores { get; set; }

    /// <summary>
    /// String representation of when the client joined the server
    /// </summary>
    private string _startTime;

    /// <summary>
    /// String representation of when the client disconnects from the server
    /// </summary>
    private string _endTime;

    /// <summary>
    /// Game ID for webserver
    /// </summary>
    private int _gameId;

    private DatabaseConnection _dbc = new DatabaseConnection();

    private readonly object _worldLock = new();

    private bool _connectedToDatabase;


    /// <summary>
    /// Disconnect the network object from the server.
    /// </summary>
    public void DisconnectFromServer()
    {
        _dataSource.Stop();
    }


    public NetworkController(IGameDataSource gamedata)
    {
        this._dataSource = gamedata;
        _dataSource.OnMessageReceived += ProcessMessage;
        _maxScores = new();
    }

    /// <summary>
    /// Default constructor - needed for Blazor initialization
    /// </summary>
    public NetworkController()
    {
        // Will be initialized when UseMockServer or UseTcpServer is called
        _maxScores = new Dictionary<int, int>();
    }

    /// <summary>
    /// Use the mock server (no network needed!)
    /// </summary>
    public void UseMockServer(string playerName)
    {
        _connectedToDatabase = false;
        _dataSource = new MockGameDataSource(playerName);
        _dataSource.OnMessageReceived += ProcessMessage;
        ConnectToServer(playerName);
    }

    /// <summary>
    /// Use the real TCP server (requires university WiFi)
    /// </summary>
    public void UseTcpServer(string server, int port, string playerName)
    {
        _connectedToDatabase = true;
        var network = new NetworkConnection();
        _dataSource = new TcpGameDataSource(network, server, port, playerName);
        _dataSource.OnMessageReceived += ProcessMessage;
        ConnectToServer(playerName);
    }

    /// <summary>
    /// Handler for the connect button
    /// Attempt to connect to the server, then start an asynchronous loop
    /// to receive  messages.
    /// </summary>
    public void ConnectToServer(string playerName)
    {
        if (_connectedToDatabase)
        {
            try
            {
                // ---- WEB SERVER LOGIC  ----
                _startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _gameId = _dbc.AddNewGame(_startTime);
                _playerName = playerName;
                _maxScores = new Dictionary<int, int>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to server: {e.Message}");
            }
        }

        // ---- GAME SERVER CONNECTION  ----
        _dataSource.Start();
    }

    private void ProcessMessage(string message)
    {
        _messageReceived = message;
     

        if (!_havePlayerId)
            SetUpId();
        else if (!_haveWorldSize)
            SetUpWorld();
        else if (!_haveAllWalls)
            SetUpWalls();
        else
            HandleJson();
    }

    /// <summary>
    /// Receives json from server and updates world model accordingly. This is used for snakes and powerups
    /// </summary>
    private void HandleJson()
    {
        //Try to parse the mystery object as a snake!
        try
        {
            if (_messageReceived.Contains("snake"))
            {
                Snake snake = JsonSerializer.Deserialize<Snake>(_messageReceived)!;

                lock (_worldLock)
                {
                    //If our world contains snake sent from server
                    if (_world.snakes.ContainsKey(snake.snakeId))
                    {
                        if (snake.dc) //If snake disconnected, remove from world
                        {
                            _world.snakes.Remove(snake.snakeId);

                            if (_connectedToDatabase)
                            {
                                _dbc.SetPlayerEndTime(snake.snakeId, _gameId,
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                if (snake.snakeId == _playerId)
                                {
                                    _dbc.SetGameEndTime(_gameId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }
                        }
                        else
                        {
                            _world.snakes[snake.snakeId] =
                                snake; //Snake in world already, so just update it in dictionary
                        }
                    }
                    else
                    {
                        _world.snakes.Add(snake.snakeId, snake);

                        // Initialize max score for this snake
                        _maxScores[snake.snakeId] = snake.score;
                        if (_connectedToDatabase)
                        {
                             _dbc.AddNewPlayer(snake.snakeId, _gameId, snake.name, snake.score, _startTime);
                        }
                    }
                }
                
                //update own snake
                if (_world.snakes.TryGetValue(_playerId, out var worldSnake))
                {
                    MySnake = worldSnake;
                }

                UpdateSnakeMaxScores(); //update max scores
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling snake json: {e.Message}" + "\n"
                 + "JSON Received:" +  _messageReceived);
        }

        try
        {
            if (_messageReceived.Contains("power"))
            {
                Powerup pow = JsonSerializer.Deserialize<Powerup>(_messageReceived)!;

                lock (_worldLock)
                {
                    if (_world.powerups.ContainsKey(pow.powerupId)) //If powerup is already in world
                    {
                        if (pow.died) //If it died, remove it from world
                        {
                            _world.powerups.Remove(pow.powerupId);
                        }
                        else
                            _world.powerups[pow.powerupId] = pow;
                    }
                    else //Powerup not in world, add it
                        _world.powerups.Add(pow.powerupId, pow);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling power json: {e.Message}");
        }
    }

    /// <summary>
    /// Updates world.walls based off of the json messages sent from the server. 
    /// </summary>
    private void SetUpWalls()
    {
        try
        {
            if (_messageReceived.Contains("wall"))
            {
                Wall? wall = JsonSerializer.Deserialize<Wall>(_messageReceived); //Throws if not a wall
                lock (_worldLock)
                {
                    //Check wall is a wall
                    if (wall is not null)
                    {
                        _world.walls.Add(wall);
                        return;
                    }
                }
            }
            else
            {
                throw new Exception("Error Setting up walls");
            }
        }
        catch
        {
            _haveAllWalls = true;
            // Now process this message as a normal game object (snake/powerup)
            HandleJson();
        }
    }
    
    /// <summary>
    /// Sets up player ID based off Json sent from server
    /// </summary>
    private void SetUpId()
    {
        if (int.TryParse(_messageReceived, out int parsedId))
        {
            _playerId = parsedId;
            _havePlayerId = true;
            MySnake = new Snake();
            MySnake.snakeId = parsedId;
        }
        else Console.WriteLine("Something went wrong with Player ID");
    }

    /// <summary>
    /// Sets world size based off server Json
    /// </summary>
    private void SetUpWorld()
    {
        //Second message, gets world size
        if (int.TryParse(_messageReceived, out int parsedSize))
        {
            lock (_worldLock)
            {
                _world = new World(parsedSize);
            }

            _haveWorldSize = true;
        }
        else Console.WriteLine("Something went wrong with Receiving World size");
    }

    /// <summary>
    /// Sends a move command to the Server!
    /// </summary>
    /// <param name="direction">direction to move!</param>
    public void HandleKeyPress(string key)
    {
        ControlCommand command = null;
        if (key == "w" || key == "ArrowUp")
            command = new ControlCommand("up");
        else if (key == "s" || key == "ArrowDown")
            command = new ControlCommand("down");
        else if (key == "a" || key == "ArrowLeft")
            command = new ControlCommand("left");
        else if (key == "d" || key == "ArrowRight")
            command = new ControlCommand("right");

        if (command != null)
        {
            _dataSource.Send(JsonSerializer.Serialize(command) + "\n");
        }
    }

    /// <summary>
    /// Updates max score dictionary based off of current state of the game
    /// </summary>
    private void UpdateSnakeMaxScores()
    {
        //update snake dictionary
        foreach (Snake s in _world.snakes.Values)
        {
            if (_maxScores.ContainsKey(s.snakeId)) //Check if snake already in dictionary
            {
                //if snakes current score is higher than max, update max
                if (_maxScores[s.snakeId] < s.score)
                {
                    _maxScores[s.snakeId] = s.score;
                    if (_connectedToDatabase)
                    {
                         _dbc.UpdatePlayerScore(s.snakeId, _gameId, s.score); //update database
                    }
                   
                }
            }
            else //Snake is not in our dictionary yet!
            {
                _maxScores.Add(s.snakeId, s.score);
                if (_connectedToDatabase)
                {
                      _dbc.UpdatePlayerScore(s.snakeId, _gameId, s.score);
                }
              
            }
        }
    }

    public World? GetWorldSnapshot()
    {
        lock (_worldLock)
        {
            if (_world == null)
                return null;

            return new World(_world); // deep copy
        }
    }
}