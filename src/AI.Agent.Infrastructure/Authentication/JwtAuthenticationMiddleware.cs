using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AI.Agent.Infrastructure.Authentication;

namespace AI.Agent.Infrastructure.Authentication;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtService _jwtService;

    public JwtAuthenticationMiddleware(RequestDelegate next, IJwtService jwtService)
    {
        _next = next;
        _jwtService = jwtService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            var principal = _jwtService.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
        }

        await _next(context);
    }
} 