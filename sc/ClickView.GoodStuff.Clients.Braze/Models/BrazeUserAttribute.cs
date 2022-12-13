namespace ClickView.GoodStuff.Clients.Braze.Models;

public class BrazeUserAttribute
{
    /// <summary>
    /// External User ID
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
    /// User First Name
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// User Last Name
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// User Email
    /// </summary>
    public string? Email { get; set; }
}