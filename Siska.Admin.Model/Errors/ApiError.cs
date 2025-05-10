namespace Siska.Admin.Model.Errors
{
    public class ApiError
    {
        public ApiError(string? id, string? message, string? innerMessage, string? stackTrace)
        {
            Id = id;
            Message = message;
#if DEBUG
            InnerMessage = innerMessage;
            StackTrace = stackTrace;
#else
            InnerMessage = null;
            StackTrace = null;
#endif
        }

        public string? Id { get; set; }
        public string? Message { get; set; }
        public string? InnerMessage { get; set; }
        public string? StackTrace { get; set; }
    }
}
