using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(ILogger<AuthenticationMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var requestData = await context.GetHttpRequestDataAsync();
        
        if (requestData != null)
        {
            // Try to get the Authorization header safely
            string? authorizationHeader = null;
            if (requestData.Headers.TryGetValues("Authorization", out var headerValues))
            {
                authorizationHeader = headerValues.FirstOrDefault();
            }
            
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                _logger.LogWarning("No Authorization header found");
                await CreateUnauthorizedResponse(context, requestData);
                return;
            }

            // Extract the token
            var token = authorizationHeader.Replace("Bearer ", string.Empty);
            
            try
            {
                // Validate the token using the configured JWT Bearer authentication
                var claims = await ValidateTokenAsync(token, context);
                
                if (claims == null)
                {
                    _logger.LogWarning("Token validation failed");
                    await CreateUnauthorizedResponse(context, requestData);
                    return;
                }

                // Set the user claims in the context
                context.Items["User"] = new ClaimsPrincipal(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                await CreateUnauthorizedResponse(context, requestData);
                return;
            }
        }

        await next(context);
    }

    private async Task<ClaimsIdentity?> ValidateTokenAsync(string token, FunctionContext context)
    {
        // This is a simplified version - in production, use proper JWT validation
        // The actual validation happens through the JwtBearer middleware configuration
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            // Basic validation - audience and issuer
            if (!jwtToken.Audiences.Contains("https://loan-system-test-functionapp-catalogue.azurewebsites.net/"))
            {
                return null;
            }

            if (jwtToken.Issuer != "https://dev-4csj4ue5kq6nzim1.us.auth0.com/")
            {
                return null;
            }

            // Check expiration
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                return null;
            }

            return new ClaimsIdentity(jwtToken.Claims, "Bearer");
        }
        catch
        {
            return null;
        }
    }

    private async Task CreateUnauthorizedResponse(FunctionContext context, HttpRequestData requestData)
    {
        var response = requestData.CreateResponse(HttpStatusCode.Unauthorized);
        await response.WriteStringAsync("Unauthorized");
        
        var invocationResult = context.GetInvocationResult();
        invocationResult.Value = response;
    }
}
