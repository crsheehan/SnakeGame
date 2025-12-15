namespace GUI.Client.Models;

/// <summary>
/// Control commands are how the client will tell the server what it wants to do (what direction it wants to move). 
/// </summary>
public class ControlCommand
{
    /// <summary>
    /// a string representing whether the player wants to move or not, and the desired direction. Possible values are: "none", "up", "left", "down", "right".
    /// </summary>
    public string moving { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ControlCommand()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="moving"></param>
    public ControlCommand(string moving)
    {
        this.moving = moving;
    }
}