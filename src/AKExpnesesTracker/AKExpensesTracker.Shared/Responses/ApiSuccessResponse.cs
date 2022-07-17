namespace AKExpensesTracker.Shared.Responses;

public class ApiSuccessResponse<T> : ApiResponse
{

    public ApiSuccessResponse()
    {
        IsSuccess = true;
        ResponseDate = DateTime.UtcNow;
    }

    public ApiSuccessResponse(IEnumerable<T>? record) : this()
    {
        Record = record;
    }

    public ApiSuccessResponse(string message, IEnumerable<T> record) : this()
    {
        Message = message;
        Record = record;
    }

    public ApiSuccessResponse(string message) : this()
    {
        Message = message;
    }

    public IEnumerable<T>? Record { get; set; }
}
