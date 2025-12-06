using epl_backend.Data.Repositories.Implementations;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Services.Implementations;
using epl_backend.Services.Interfaces;

namespace epl_backend.Startup;

public static class DependenciesConfig
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        // repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IClubRepository, ClubRepository>();

        // login service 
        builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
    }
}
