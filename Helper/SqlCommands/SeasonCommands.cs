namespace epl_backend.Helper.SqlCommands;

public static class SeasonCommands
{
    public static string AddSeasonCommand => "PL_AddSeason";
    public static string UpdateSeasonCommand => "PL_UpdateSeason";
    public static string DeleteSeasonCommand => "PL_DeleteSeason";
    public static string GetAllSeasonCommand => "PL_GetAllSeason";
    public static string FindSeasonByIdCommand => "PL_FindSeasonById";
    public static string SeasonExistingCommand => "PL_SeasonExisting";
    public static string CountSeasonCommand => "PL_CountSeason";
}
