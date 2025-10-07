namespace UAE_Pass_Poc.Exceptions
{
    public class UaePassRequestException : Exception
    {
        
        public string? ErrorCode { get; set; }
        public UaePassRequestException(string message, string? errorCode = null) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}