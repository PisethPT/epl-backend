using epl_backend.Data;
using epl_backend.Data.Repositories.Implementations;
using epl_backend.Data.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Minimal hosting model used, so register repositories here
builder.Services.AddScoped<IClubRepository, ClubRepository>();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
