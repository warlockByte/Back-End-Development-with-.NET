using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using UserManagementAPI.Configuration;
using UserManagementAPI.Models;

namespace UserManagementAPI.Middleware;

public sealed class TokenAuthenticationMiddleware(
    RequestDelegate next,
    IOptions<ApiTokenOptions> options,
    ILogger<TokenAuthenticationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var authorization = context.Request.Headers.Authorization.ToString();
        const string bearerPrefix = "Bearer ";

        var tokenIsValid = authorization.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
            && IsValidToken(authorization[bearerPrefix.Length..].Trim(), options.Value.ApiToken);

        if (!tokenIsValid)
        {
            logger.LogWarning("Unauthorized request: {Method} {Path} returned {StatusCode}",
                context.Request.Method, context.Request.Path, StatusCodes.Status401Unauthorized);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers.WWWAuthenticate = "Bearer";
            await context.Response.WriteAsJsonAsync(new ErrorResponse("Unauthorized."));
            return;
        }

        await next(context);
    }

    private static bool IsValidToken(string providedToken, string configuredToken)
    {
        var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(providedToken));
        var configuredHash = SHA256.HashData(Encoding.UTF8.GetBytes(configuredToken));
        return CryptographicOperations.FixedTimeEquals(providedHash, configuredHash);
    }
}
