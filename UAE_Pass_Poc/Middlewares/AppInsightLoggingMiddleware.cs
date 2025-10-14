using System.Text.RegularExpressions;

namespace UAE_Pass_Poc.Middlewares
{
    public class AppInsightLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AppInsightLoggingMiddleware> _logger;

        public AppInsightLoggingMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory?.CreateLogger<AppInsightLoggingMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Capture the request body
            context.Request.EnableBuffering();

            string requestBody = await SanitizeRequestForLogging(context);

            context.Request.Body.Position = 0; // Reset the request body stream

            // Log request
            _logger.LogInformation("HTTP {Method} Path:{RequestPath} RequestBody: {RequestBody}", context.Request.Method, context.Request.Path, requestBody);

            // Capture the response body
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context); // Proceed with the request pipeline

            // Capture response content
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Log response 
            responseBody = SanitizeBase64Fields(responseBody);
            _logger.LogInformation("HTTP {Method} Path:{RequestPath} Response Body: {ResponseBody}", context.Request.Method, context.Request.Path, responseBody);

            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }

        private static async Task<string> SanitizeRequestForLogging(HttpContext context)
        {
            string requestBody = string.Empty;
            if (context.Request.HasFormContentType)
            {
                var form = await context.Request.ReadFormAsync();
                var sanitizedForm = new Dictionary<string, string>();

                foreach (var field in form)
                {
                    sanitizedForm[field.Key] = field.Value.ToString();
                }

                foreach (var field in form.Files)
                {
                    sanitizedForm[field.Name] = $"[FILE REMOVED: Name={field.FileName}, Size={field.Length} bytes, ContentType={field.ContentType}]";
                }

                requestBody = string.Join(", ", sanitizedForm.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            }
            else
            {
                requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                requestBody = SanitizeBase64Fields(requestBody);
            }

            return requestBody;
        }

        private static string SanitizeBase64Fields(string requestBody)
        {
            var base64Regex = new Regex(@"(?:[A-Za-z0-9+/]{4}){1000,}(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?",
                                        RegexOptions.Compiled);
            return base64Regex.Replace(requestBody, "[REDACTED BASE64]");
        }
    }
}