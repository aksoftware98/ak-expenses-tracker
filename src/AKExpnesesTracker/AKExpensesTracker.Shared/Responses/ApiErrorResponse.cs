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
}
