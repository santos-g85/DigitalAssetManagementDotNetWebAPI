public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public static ServiceResult<T> Success(T data, string message, int statusCode) => new()
    {
        IsSuccess = true,
        Data = data,
        Message = message,
        StatusCode = statusCode
    };
    public static ServiceResult<T> Failure(string error, int statusCode) => new()
    {
        IsSuccess = false,
        Message = error,
        StatusCode = statusCode
    };
}
