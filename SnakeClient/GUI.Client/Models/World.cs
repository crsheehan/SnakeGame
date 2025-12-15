namespace GUI.Client.Models;

/// <summary>
/// A representation of the world beign drawn
/// </summary>
public class World
{ 
    // Dictionary of snakes and their ID's
    public Dictionary<int,Snake> snakes { get; set; }

    //Dictionary of powerups and their IDS
     public Dictionary<int,Powerup> powerups { get; }
     
     //List of walls
     public List<Wall> walls { get; }

     /// <summary>
     /// Size of world
     /// </summary>
     public int size {get; set;}
     
    /// <summary>
    /// Default constructor
    /// </summary>
    public World()
    {
        snakes = new Dictionary<int,Snake>();
        powerups = new Dictionary<int, Powerup>();
        walls = new List<Wall>();
        
    }

    /// <summary>
    /// Constructor with world size
    /// </summary>
    /// <param name="size">size</param>
    public World(int size)
    {
        snakes = new Dictionary<int, Snake>();
        powerups = new Dictionary<int, Powerup>();
        walls = new List<Wall>();
        this.size = size;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="world"></param>
    public World(World world)
    {
        this.powerups = world.powerups;
        this.snakes = world.snakes;
        this .walls = world.walls;
        this.size = world.size;
    }


  
}