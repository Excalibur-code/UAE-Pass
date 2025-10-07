namespace UAE_Pass_Poc.ResponseHandlers.Models
{
    public interface IError
    {
        string? ErrorCode { get; set; }
        string? ErrorDescription { get; set; }
    }

    public interface IRestResult
    {
        int StatusCode { get; set; }
    }

    public interface IErrorResult : IRestResult
    {
        IError? Error { get; set; }
    }

    public interface ISuccessResult : IRestResult
    {
        string? Message { get; set; }
    }

    public interface ISuccessResult<TData> : ISuccessResult
    {
        TData? Data { get; set; }
    }

    public interface IRestResult<TData> : ISuccessResult<TData>, IErrorResult
    {

    }
}