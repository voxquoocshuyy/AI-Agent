using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace AI.Agent.IntegrationTests;

public abstract class TestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    protected TestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Add test services here
                ConfigureTestServices(services);
            });
        });
        Client = Factory.CreateClient();
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        // Override in derived classes to configure test services
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Configure test services here
        });

        return base.CreateHost(builder);
    }
} 