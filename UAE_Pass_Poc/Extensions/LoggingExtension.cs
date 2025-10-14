using Serilog;
using Serilog.Exceptions;
using UAE_Pass_Poc.Middlewares;

namespace UAE_Pass_Poc.Extensions
{
    public static class LoggingExtension
    {
        public static WebApplicationBuilder AddLogger(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                .Enrich.WithExceptionDetails()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Host.UseSerilog();

            return builder;
        }

        public static WebApplication UseLogger(this WebApplication app)
        {
            app.UseSerilogRequestLogging();
            app.UseMiddleware<AppInsightLoggingMiddleware>();
            return app;
        }
    }
}