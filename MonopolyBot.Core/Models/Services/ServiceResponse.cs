using MonopolyBot.Core.Enums;

namespace MonopolyBot.Core.Models.Services
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public ErrorType ErrorType { get; set; } = ErrorType.None;

        public ServiceResponse(bool success, string message, T? data, ErrorType errorType)
        {
            Success = success;
            Message = message;
            Data = data;
            ErrorType = errorType;
        }
        public ServiceResponse() { }
    }
}
