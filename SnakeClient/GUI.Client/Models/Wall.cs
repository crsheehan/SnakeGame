namespace GUI.Client.Models;

/// <summary>
/// Represents a wall in the snake game
/// </summary>
public class Wall
{
    /// <summary>
    /// ID of this individual wall
    /// </summary>
    public int wall{get; set;}
    /// <summary>
    /// A Point2D representing one endpoint of the wall
    /// </summary>
    public Point2D p1{get; set;}
    
    /// <summary>
    /// A Point2D representing the other endpoint of the wall. 
    /// </summary>
    public Point2D p2{get; set;}

    public Wall(int id, Point2D p1, Point2D p2)
    {
        this.wall = id;
        this.p1 = p1;
        this.p2 = p2;
    }

    /// <summary>
    /// Deafault constructor
    /// </summary>
    public Wall()
    {
        
    }
}