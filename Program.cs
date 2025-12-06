using epl_backend.Data;
using epl_backend.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using static epl_backend.Startup.DependenciesConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/auth/access-denied";
    });

// Minimal hosting model used, so register repositories here
builder.RegisterServices();

var app = builder.Build();

AppDbContext.Initialize(app.Configuration);
app.Lifetime.ApplicationStopped.Register(() =>
{
    try
    {
        AppDbContext.Instance.Dispose();
    }
    catch
    {
        // ignored
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Use custom middleware BEFORE routing
app.UseMiddleware<ValidateUserMiddleware>();

app.MapGet("/", context =>
{
    context.Response.Redirect("/auth/login");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
.WithStaticAssets();

app.Run();
