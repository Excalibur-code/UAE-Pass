namespace UAE_Pass_Poc.ResponseHandlers.Models
{
    public class ErrorItem : IError
    {
        public string? ErrorCode { get; set; }
        public string? ErrorDescription { get; set; }
        public string? ErrorDetail { get; internal set; }
    }

    internal class RestResult : IRestResult
    {
        public int StatusCode { get; set; }
        public RestResult(int statusCode)
        {
            StatusCode = statusCode;
        }
    }

    internal class RestResult<TData> : RestResult, IRestResult<TData>
    {
        public string? Message { get; set; }
        public TData? Data { get; set; }
        public IError? Error { get; set; }

        public RestResult(int statusCode) : base(statusCode) 
        {

        }
    }

    internal class SuccessResult<TData> : RestResult, ISuccessResult<TData>
    {
        public SuccessResult() : base(200)
        {

        }
        public string? Message { get; set; }
        public TData? Data { get; set; }
    }

    internal class ErrorResult : RestResult, IErrorResult
    {
        public ErrorResult(int statusCode) : base(statusCode)
        {

        }
        public IError? Error { get; set; }
    }

    //internal class RestResult<TData> : RestResult, ISuccessResult<TData>, IErrorResult
    //{
    //    public RestResult(int statusCode) : base(statusCode)
    //    {

    //    }
    //    public string? Message { get; set; }
    //    public TData? Data { get; set; }
    //    public IError? Error { get; set; }
    //}

}
