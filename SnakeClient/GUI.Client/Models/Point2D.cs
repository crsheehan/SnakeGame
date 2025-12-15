namespace GUI.Client.Models;

/// <summary>
/// Represents a 2D point in space, an (x,y) pair. These are WORLD-SPACE LOCATIONS
/// </summary>
/// <param name="x">Int x coordinate</param>
/// <param name="y">Int y coordinate</param>
public class Point2D
{
    /// <summary>
    /// X coordinate
    /// </summary>
    public int X {get; set;}

    /// <summary>
    /// Y coordinate
    /// </summary>
    public int Y {get; set;}

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Point2D(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public Point2D()
    {
        this.X = X;
        this.Y = Y;
    }
   
}