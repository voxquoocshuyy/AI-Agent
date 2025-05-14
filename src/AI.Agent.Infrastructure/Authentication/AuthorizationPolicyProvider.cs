using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AI.Agent.Infrastructure.Authentication;

public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
    private IConfiguration Configuration { get; }

    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IConfiguration configuration)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        Configuration = configuration;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        var policy = new AuthorizationPolicyBuilder();
        
        switch (policyName)
        {
            case "RequireAdminRole":
                policy.RequireRole("Admin");
                break;
            case "RequireUserRole":
                policy.RequireRole("User");
                break;
            case "RequireApiKey":
                policy.RequireClaim("ApiKey");
                break;
            default:
                return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        return Task.FromResult<AuthorizationPolicy?>(policy.Build());
    }
} 