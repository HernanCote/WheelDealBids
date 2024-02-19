namespace BuildingBlocks.Utils;

public class Error
{
    /// <summary>
    /// Gets or sets the code for this error.
    /// </summary>
    /// <value>
    /// The code for this error.
    /// </value>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the description for this error.
    /// </summary>
    /// <value>
    /// The description for this error.
    /// </value>
    public string? Message { get; set; }

    public Error() { }
    
    public Error(string code, string? message = null)
    {
        Code = Code;
        Message = Message;
    }
    
    public static Error BadRequest(string? message = null)
    {
        return From("Error.BadRequest", message ?? "Bad request.");
    }   
    
    public static Error From(string code, string message)
    {
        return new Error(code, message);
    }
}
