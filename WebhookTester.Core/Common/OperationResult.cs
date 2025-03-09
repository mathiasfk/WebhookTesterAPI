namespace WebhookTester.Core.Common
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public Error? Error { get; set; }

        protected OperationResult(bool success, Error? error = null)
        {
            Success = success;
            Error = error;
        }

        public static OperationResult SuccessResult() => new(true);
        public static OperationResult FailureResult(string errorMessage, ErrorCode errorCode) => new(false, new Error { Code = errorCode, Message = errorMessage });
    }

    public class OperationResult<T> : OperationResult
    {
        public T Data { get; }

        private OperationResult(T data) : base(true)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        private OperationResult(string errorMessage, ErrorCode errorCode) : base(false, new Error { Code = errorCode, Message = errorMessage })
        {
            Data = default!;
        }

        public static OperationResult<T> SuccessResult(T data) => new(data);
        public static new OperationResult<T> FailureResult(string errorMessage, ErrorCode errorCode) => new(errorMessage, errorCode);
    }
}
