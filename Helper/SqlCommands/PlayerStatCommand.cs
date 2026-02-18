namespace PremierLeague_Backend.Helper.SqlCommands;

public static class PlayerStatCommand
{
    public static string GetPlayerStatMatchListCommand => "PL_GetPlayerStatMatchList";
    public static string GetStatByCategoryIdForPlayerStatCommand => "PL_GetStatByCategoryIdForPlayerStat";
    public static string GetHomePlayersForPlayerStatByClubIdAndMatchIdCommand => "PL_GetHomePlayersForPlayerStatByClubIdAndMatchId";
    public static string GetAwayPlayersForPlayerStatByClubIdAndMatchIdCommand => "PL_GetAwayPlayersForPlayerStatByClubIdAndMatchId";
}
