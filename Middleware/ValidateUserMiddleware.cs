using epl_backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace epl_backend.Middleware;

public class ValidateUserMiddleware
{
    private readonly RequestDelegate _next;

    public ValidateUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository repository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await repository.FindByIdAsync(userId);
                if (user != null)
                {
                    bool isLocked = user.LockoutEnabled 
                && user.LockoutEnd.HasValue 
                && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

                    if (isLocked)
                    {
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Response.Redirect("/auth/login");
                        return;
                    }
                }
                else
                {
                    // User not found → logout
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Response.Redirect("/auth/login");
                    return;
                }
            }
        }

        await _next(context);
    }
}