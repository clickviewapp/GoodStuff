namespace ClickView.GoodStuff.Clients.Braze.Models;

public class BrazePurchase
{
    public BrazePurchase(string productId, string currency, float price, DateTime time)
    {
        ProductId = productId;
        Currency = currency;
        Price = price;
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
    /// Identifier for the purchase e.g. Product Name or Product Category
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// ISO 4217 Alphabetic Currency Code
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Value in the base currency unit (e.g., Dollars for USD, Yen for JPY)
    /// </summary>
    public float Price { get; set; }

    /// <summary>
    /// The quantity purchased (defaults to 1, must be less than or equal to 100)
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Time of purchase
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// Properties of the event
    /// </summary>
    public Dictionary<string, object>? Properties { get; set; }
}