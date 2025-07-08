using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; }
    public int StatusCode { get; set; }

    // Constructor
    public ApiResponse(bool success, string message, T? data = default, int statusCode = 400)
    {
        Success = success;
        Message = message;
        Data = data;
        StatusCode = statusCode;
        Errors = new List<string>();
    }

    // Method to populate the response for success
    public static ApiResponse<T> SuccessResponse(T data, string message = " ", int statusCode=200)
    {
        return new ApiResponse<T>(true, message, data, statusCode);
    }

    // Method to populate the response for failure
    public static ApiResponse<T> FailureResponse(string message, int statusCode = 400)
    {
        return new ApiResponse<T>(false, message, default, statusCode);
    }

    // Optional: Helper method to add errors dynamically
    public void AddError(string error)
    {
        Errors.Add(error);
    }

    // Optional: Static method to create an error response with error list
    public static ApiResponse<T> FailureResponseWithErrors(List<string> errors,  int statusCode = 400)
    {
        var response = new ApiResponse<T>(false, "Operation failed", default, statusCode);
        response.Errors = errors ?? new List<string>();
        return response;
    }
}
