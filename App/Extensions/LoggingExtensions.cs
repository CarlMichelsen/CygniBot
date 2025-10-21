using Serilog;

namespace App.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ApplicationUseSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, sp, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(sp)
                .Enrich.WithProperty("Application", ApplicationConstants.Name)
                .Enrich.WithProperty("Version", ApplicationConstants.Version)
                .Enrich.WithProperty("Environment", GetEnvironmentName(builder.Environment));
        });

        return builder;
    }
    
    private static string GetEnvironmentName(IHostEnvironment environment) =>
        environment.IsProduction() ? "Production" : "Development";
}