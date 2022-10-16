namespace Shared.Message.Responses;

public class ErrorResponse
{
    public string Message { get; set; }

    public ErrorResponse()
    {
        
    }
    
    public ErrorResponse(string message)
    {
        this.Message = message;
    }
}