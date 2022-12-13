namespace ClickView.GoodStuff.Clients.Braze.Models;

public class BrazeEvent
{
    public BrazeEvent(string name, DateTime time)
    {
        Name = name;
        Time = time;
    }

    /// <summary>
    ///  External User ID
    /// </summary>
    public string? ExternalId { get; set; }
    
    /// <summary>
    /// User Alias Object
    /// </summary>
    public BrazeUserAlias? UserAlias { get; set; }
    
    /// <summary>
    /// Braze User Identifier
    /// </summary>
    public string? BrazeId { get; set; }
    
    /// <summary>
    /// App Identifier
    /// </summary>
    public string? AppId { get; set; }
    
    /// <summary>
    /// The name of the event
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The time of the event
    /// </summary>
    public DateTime Time { get; set; }
    
    /// <summary>
    /// Properties of the event
    /// </summary>
    public Dictionary<string, object>? Properties { get; set; }
}