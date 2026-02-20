namespace GuardianStock.API.Extensions;

public sealed class OpenTelemetrySettings
{
    public const string SectionName = "OpenTelemetry";

    public string ServiceName { get; init; } = "GuardianStock.API";
    public string Endpoint { get; init; } = "http://localhost:4317";
}
