using PremierLeague_Backend.Data.Repositories.Implementations;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Services.Implementations;
using PremierLeague_Backend.Services.Interfaces;

namespace PremierLeague_Backend.Startup;

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
        builder.Services.AddScoped<IGoalRepository, GoalRepository>();
        builder.Services.AddScoped<ILineupRepository, LineupRepository>();
        builder.Services.AddScoped<IPlayerStatRepository, PlayerStatRepository>();

        // select list item
        builder.Services.AddScoped<ISelectListItems, SelectListItems>();

        // login service 
        builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
    }
}
