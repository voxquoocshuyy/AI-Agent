using AI.Agent.Domain.Repositories;
using AI.Agent.Infrastructure.HealthChecks;
using AI.Agent.Infrastructure.Logging;
using AI.Agent.Infrastructure.Persistence;
using AI.Agent.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AI.Agent.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Add Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        // Add Health Checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("Database", tags: new[] { "ready" })
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!, name: "PostgreSQL", tags: new[] { "ready" });

        // Add Logging
        services.AddLoggingServices(configuration);

        return services;
    }
} 