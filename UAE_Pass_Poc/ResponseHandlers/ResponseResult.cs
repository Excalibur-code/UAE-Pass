

using UAE_Pass_Poc.ResponseHandlers.Models;

namespace UAE_Pass_Poc.ResponseHandlers
{
    public static class ResponseResult
    {
        public static ISuccessResult<string> SuccessMessage(string? message = null)
        {
            return new SuccessResult<string>
            {
                Message = message
            };
        }

        public static ISuccessResult<TData> Success<TData>(TData data, string? message = null)
        {
            return new SuccessResult<TData>()
            {
                Data = data,
                Message = message
            };
        }

        public static IErrorResult Error(string error, string? errorCode = "BadRequest", int statusCode = 400, string? detail = null)
        {
            return new ErrorResult(statusCode)
            {
                Error = new ErrorItem
                {
                    ErrorDescription = error,
                    ErrorCode = errorCode,
                    ErrorDetail = detail
                }
            };
        }
    }

}
