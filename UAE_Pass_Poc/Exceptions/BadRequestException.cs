namespace UAE_Pass_Poc.Exceptions
{
    public class BadRequestException : Exception
    {
        public string? ErrorCode { get; set; }
        public BadRequestException(string message, string? errorCode = null) : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    public class InternalErrorException : Exception
    {
        public int StatusCode { get; set; }
        public string? ErrorCode { get; set; }
        public InternalErrorException(string message, int statusCode = 500, string? errorCode = null) : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}