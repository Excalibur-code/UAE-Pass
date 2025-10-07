using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UAE_Pass_Poc.Exceptions;
using UAE_Pass_Poc.ResponseHandlers;

namespace UAE_Pass_Poc.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class FormatExceptionResponseAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<FormatExceptionResponseAttribute> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FormatExceptionResponseAttribute(ILogger<FormatExceptionResponseAttribute> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            //Handle Timeout Exception
            if (context.Exception is TaskCanceledException ext && context.Exception.Message.Contains("HttpClient.Timeout"))
            {
                _logger.LogError(context.Exception, "{Message}", ext.Message);
                context.Result = new ObjectResult(ResponseResult.Error("Unexpected error while processing request. Please try again later.", "Internal Error", 500)) { StatusCode = 500 };
            }
            else if (context.Exception is BadRequestException ex)
            {
                _logger.LogError(context.Exception, "{Message}", ex.Message);
                context.Result = new BadRequestObjectResult(ResponseResult.Error(ex.Message, ex.ErrorCode));
            }
            else if (context.Exception is InternalErrorException exx)
            {
                _logger.LogError(context.Exception, "{Message}", exx.Message);
                context.Result = new ObjectResult(ResponseResult.Error(context.Exception.Message, exx.ErrorCode, 500)) { StatusCode = exx.StatusCode };
            }
            else if (context.Exception is ArgumentNullException)
            {
                _logger.LogError(context.Exception, "{Message}", context.Exception.Message);
                context.Result = new BadRequestObjectResult(ResponseResult.Error(context.Exception.Message, "ArgumentNullException"));
            }
            else if(context.Exception is UaePassRequestException uaeEx)
            {
                _logger.LogError(context.Exception, "{Message}", context.Exception.Message);
                context.Result = new ObjectResult(ResponseResult.Error(uaeEx.Message, uaeEx.ErrorCode, 500)) { StatusCode = 500 };
            }
            else
            {
                _logger.LogError(context.Exception, "{Message}", context.Exception.Message);
                context.Result = new ObjectResult(ResponseResult.Error(
                    _webHostEnvironment.IsDevelopment() ? context.Exception.Message : "Something went wrong! please try again.",
                    "Internal Error",
                    500)
                    )
                {
                    StatusCode = 500
                };
            }
        }
    }
}
