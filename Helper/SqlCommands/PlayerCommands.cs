namespace PremierLeague_Backend.Helper.SqlCommands;

public static class PlayerCommands
{
    public static string AddPlayerCommand => "PL_AddPlayer";
    public static string UpdatePlayerCommand => "PL_UpdatePlayer";
    public static string DeletePlayerCommand => "PL_DeletePlayer";
    public static string GetAllPlayerCommand => "PL_GetAllPlayer";
    public static string  FindPlayerByIdCommand => "PL_FindPlayerById";
    public static string PlayerExistingByClubCommand => "PL_PlayerExistingByClub";
    public static string CountPlayersClubCommand => "PL_CountPlayers";
}
