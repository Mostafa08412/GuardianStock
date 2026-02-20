using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GuardianStock.API.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(OpenTelemetrySettings.SectionName).Get<OpenTelemetrySettings>()
                       ?? new OpenTelemetrySettings();

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: settings.ServiceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddOtlpExporter(options => options.Endpoint = new Uri(settings.Endpoint)))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options => options.Endpoint = new Uri(settings.Endpoint)));

        return services;
    }
}
