using System.Text.Json.Serialization;

namespace GUI.Client.Models;

/// <summary>
/// Model of snake
/// </summary>
public class Snake
{
    /// <summary>
    /// Constructor
    /// </summary>
    public Snake()
    {
        snakeId = snakeId;
        name = name;
        body = new List<Point2D>();
        dir = new Point2D();
        score = score;
        died = died;
        alive = alive;
        dc = dc;
        join = join;

    }
    [JsonPropertyName("snake")]
    // An int representing the snake's unique ID.  
    public int snakeId {get; set;}
    
    // A string representing the player's name.
    public string name{get; set;}

    //a List<Point2D> representing the entire body of the snake
    //The first point of the list gives the location of the snake's tail,
    //and the last gives the location of the snake's head. 
    public List<Point2D> body{get; set;}
    
    //a Point2D representing the snake's orientation
    public Point2D dir{get; set;}

    //an int representing the player's score
    public int score{get; set;}

    // a bool indicating if the snake died on this frame.
    // This will only be true on the exact frame in which the snake died. 
    public bool died{get; set;}

    //a bool indicating whether a snake is alive or dead
    public bool alive{get; set;}

    //a bool indicating if the player controlling that snake disconnected on that frame.
    //The server will send the snake with this flag set to true only once, then it will discontinue sending that snake for the rest of the game
    public bool dc{get; set;}

    // a bool indicating if the player joined on this frame. This will only be true for one frame.
    public bool join{get; set;}
    
    // /// <summary>
    // /// The max score that a snake has ever had
    // /// </summary>
    // [JsonIgnore]
    // public int maxScore { get; set; }
}