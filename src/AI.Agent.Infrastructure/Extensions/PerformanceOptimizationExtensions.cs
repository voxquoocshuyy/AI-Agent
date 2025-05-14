using System.IO.Compression;
using AI.Agent.Infrastructure.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResponseCompressionMiddleware = AI.Agent.Infrastructure.Middleware.ResponseCompressionMiddleware;

namespace AI.Agent.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring performance optimization
/// </summary>
public static class PerformanceOptimizationExtensions
{
    /// <summary>
    /// Adds performance optimization services to the application
    /// </summary>
    public static IServiceCollection AddPerformanceOptimization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Redis caching
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "AI_Agent_";
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        // Add response compression
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        return services;
    }

    /// <summary>
    /// Uses performance optimization middleware in the application pipeline
    /// </summary>
    public static IApplicationBuilder UsePerformanceOptimization(this IApplicationBuilder app)
    {
        app.UseResponseCompression();
        app.UseMiddleware<ResponseCompressionMiddleware>();

        return app;
    }
}