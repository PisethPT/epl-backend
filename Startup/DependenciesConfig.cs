using epl_backend.Data.Repositories.Implementations;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Services.Implementations;
using epl_backend.Services.Interfaces;

namespace epl_backend.Startup;

public static class DependenciesConfig
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        // execute query
        builder.Services.AddScoped<IExecute, Execute>();

        // repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IClubRepository, ClubRepository>();
        builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
        builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
        builder.Services.AddScoped<IMatchRepository, MatchRepository>();

        // select list item
        builder.Services.AddScoped<ISelectListItems, SelectListItems>();

        // login service 
        builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
    }
}
