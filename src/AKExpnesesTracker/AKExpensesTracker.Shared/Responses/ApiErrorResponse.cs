namespace AKExpensesTracker.Shared.Responses;

public class ApiErrorResponse : ApiResponse
{
    public ApiErrorResponse()
    {
        IsSuccess = false;
        ResponseDate = DateTime.UtcNow;
    }

    public ApiErrorResponse(string message) : this()
    {
        Message = message;
    }

    public ApiErrorResponse(string message, IEnumerable<string>? errors) : this()
    {
        Message = message;
        Errors = errors; 
    }

    public IEnumerable<string>? Errors { get; set; }
}
