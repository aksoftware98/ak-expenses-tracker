namespace AKExpensesTracker.Shared.Responses;

public class ApiSuccessResponse<T> : ApiResponse
{

    public ApiSuccessResponse()
    {
        IsSuccess = true;
        ResponseDate = DateTime.UtcNow;
    }

    public ApiSuccessResponse(T? record) : this()
    {
        Record = record;
    }

    public ApiSuccessResponse(string message, T record) : this()
    {
        Message = message;
        Record = record;
    }

    public ApiSuccessResponse(string message) : this()
    {
        Message = message;
    }

    public T? Record { get; set; }
}
