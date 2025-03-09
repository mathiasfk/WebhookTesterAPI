namespace WebhookTester.Core.Common
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public Error? Error { get; set; }

        public static OperationResult SuccessResult() => new() { Success = true };
        public static OperationResult FailureResult(string errorMessage, ErrorCode errorCode) => new()
        {
            Success = false,
            Error = new() { Code = errorCode, Message = errorMessage }
        };
    }

    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; set; }

        public static OperationResult<T> SuccessResult(T data) => new() { Success = true, Data = data };
        public static new OperationResult<T> FailureResult(string errorMessage, ErrorCode errorCode) => new()
        {
            Success = false,
            Error = new() { Code = errorCode, Message = errorMessage }
        };
    }
}
