using System.Diagnostics;
using System.Text.Json.Serialization;

namespace GUI.Client.Models;

public class Powerup
{
    [JsonPropertyName("power")]
    /// <summary>
    /// An int representing the powerup's unique ID
    /// </summary>
    public int powerupId { get; set; }

    /// <summary>
    /// A point2D representing the location of the powerup
    /// </summary>
    public Point2D? loc { get; set; }
    
    /// <summary>
    /// A bool idicating if the powerup "died" (was collected by a player on this frame)
    /// </summary>
    public bool died { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="power"></param>
    /// <param name="loc"></param>
    public Powerup(int power, Point2D loc)
    {
        this.powerupId = power;
        this.loc = loc;
        this.died = false;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public Powerup()
    {
        this.powerupId = powerupId;
        this.loc = new Point2D();
        this.died = false;
    }
}