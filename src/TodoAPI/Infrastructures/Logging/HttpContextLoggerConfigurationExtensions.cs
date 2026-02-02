using Serilog.Configuration;

namespace TodoAPI.Infrastructures.Logging;

public static class HttpContextLoggerConfigurationExtensions
{
    public static LoggerConfiguration WithCustomHttpContext(
        this LoggerEnrichmentConfiguration enrichmentConfiguration
    )
    {
        if (enrichmentConfiguration == null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With<CustomHttpContextEnricher>();
    }
}
