namespace PremierLeague_Backend.Helper.SqlCommands;

public static class ClubCommands
{
    public static string AddClubCommand => "PL_AddClub";
    public static string DeleteClubCommand => "PL_DeleteClub";
    public static string ClubExistByNameCommand => "PL_ClubExistByName";
    public static string GetAllClubsCommand => "PL_GetAllClubs";
    public static string GetClubByIdCommand => "PL_GetClubById";
    public static string UpdateClubCommand => "PL_UpdateClub";
}
