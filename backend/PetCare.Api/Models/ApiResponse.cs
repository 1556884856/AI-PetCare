namespace PetCare.Api.Models;
/// <summary>统一 API 响应格式，包含状态码和消息</summary>
public class ApiResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public static ApiResponse Success(string message = "操作成功") => new() { Code = 200, Message = message };
    public static ApiResponse Error(int code, string message) => new() { Code = code, Message = message };
}
/// <summary>带数据的泛型 API 响应，继承基本响应格式</summary>
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
    public static ApiResponse<T> Success(T data, string message = "操作成功") => new() { Code = 200, Message = message, Data = data };
    public new static ApiResponse<T> Error(int code, string message) => new() { Code = code, Message = message, Data = default };
}
