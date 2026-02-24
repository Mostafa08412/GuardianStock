namespace GuardianStock.API.Contracts
{
    public record ApiResponse
    {

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public string ErrorCode { get; private set; }
        public IDictionary<string, string> ValidationErrors { get; private set; } = new Dictionary<string, string>();
        public IDictionary<string, string> Meta { get; private set; } = new Dictionary<string, string>();
        public string Instance { get; private set; }
        public string TraceId { get; private set; }

        public ApiResponse(bool isSucess, string message, string errorCode, IDictionary<string, string> validationErrors, IDictionary<string, string> meta, string instance, string traceId)
        {
            IsSuccess = isSucess;
            Message = message;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors;
            Meta = meta;
            Instance = instance;
            TraceId = traceId;
        }
    }

    public record ApiResponse<T> : ApiResponse
    {
        public ApiResponse(bool isSucess, string message, string errorCode, IDictionary<string, string> validationErrors, IDictionary<string, string> meta, string instance, string traceId, T data) :
            base(isSucess, message, errorCode, validationErrors, meta, instance, traceId)
        {
            Data = data;
        }
        public T Data { get; private set; }

    }
}
