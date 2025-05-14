using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace AI.Agent.Infrastructure.Logging;

public static class LoggingConfiguration
{
    public static IServiceCollection AddLoggingServices(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticsearchUrl = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
        var kibanaUrl = configuration["Kibana:Url"] ?? "http://localhost:5601";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                IndexFormat = $"aiagent-{DateTime.UtcNow:yyyy-MM}",
                NumberOfShards = 2,
                NumberOfReplicas = 1,
                ModifyConnectionSettings = x =>
                    x.ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true),
                CustomFormatter = new ElasticsearchJsonFormatter()
            })
            .WriteTo.File(
                path: "logs/aiagent-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        services.AddLogging(loggingBuilder => { loggingBuilder.AddSerilog(dispose: true); });

        return services;
    }
}