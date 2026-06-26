namespace PetCare.Api.Models;

/// <summary>统一API响应格式</summary>
public class ApiResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse Success(string message = "操作成功")
        => new() { Code = 200, Message = message };

    public static ApiResponse Error(int code, string message)
        => new() { Code = code, Message = message };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "操作成功")
        => new() { Code = 200, Message = message, Data = data };

    public new static ApiResponse<T> Error(int code, string message)
        => new() { Code = code, Message = message, Data = default };
}
