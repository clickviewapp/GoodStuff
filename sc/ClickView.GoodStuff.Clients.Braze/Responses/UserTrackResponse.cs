namespace ClickView.GoodStuff.Clients.Braze.Responses;

public class UserTrackResponse
{
    public UserTrackResponse(string message)
    {
        Message = message;
    }

    /// <summary>
    /// If "success" then any data that was not affected by an error in the errors array will still be processed.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The errors that occured
    /// </summary>
    public IEnumerable<string>? Errors { get; set; }

    /// <summary>
    /// if attributes are included in the request,
    /// this will return an integer of the number of external_ids with attributes that were queued to be processed
    /// </summary>
    public int? AttributesProcessed { get; set; }

    /// <summary>
    /// If events are included in the request,
    /// this will return an integer of the number of events that were queued to be processed
    /// </summary>
    public int? EventsProcessed { get; set; }

    /// <summary>
    /// If purchases are included in the request,
    /// this will return an integer of the number of purchases that were queued to be processed
    /// </summary>
    public int? PurchasesProcessed { get; set; }
}