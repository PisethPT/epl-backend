namespace PremierLeague_Backend.Helper.SqlCommands;

public static class MatchCommands
{
    public static string AddMatchCommand => "PL_AddMatch";
    public static string UpdateMatchCommand => "PL_UpdateMatch";
    public static string DeleteMatchCommand => "PL_DeleteMatch";
    public static string GetAllMatchesCommand => "PL_GetAllMatches";
    public static string GetMatchWeekCommand => "PL_GetMatchWeek";
    public static string FindMatchByIdCommand => "PL_FindMatchById";
    public static string FindClubCannotBeTheSameCommand => "PL_FindClubCannotBeTheSame";
    public static string FindMatchDateIsOutsideTheActiveSeasonCommand => "PL_FindMatchDateIsOutsideTheActiveSeason";
    public static string FindMatchDateCannotBeEarlierThanTodayCommand => "PL_FindMatchDateCannotBeEarlierThanToday";
    public static string FindMatchAlreadyExistsOnTheSelectedDateCommand => "PL_FindMatchAlreadyExistsOnTheSelectedDate";
    public static string CountMatchesCommand => "PL_CountMatches";
    public static string GetMatchWeekBySeasonIdCommand => "PL_GetMatchWeekBySeasonId";
    public static string FindMatchRefereeByMatchIdCommand => "PL_FindMatchRefereeByMatchId";
    public static string GetCurrentMatchWeekAndSeasonIdCommand => "PL_GetCurrentMatchWeekAndSeasonId";
    public static string GetMatchDetailCommand => "PL_GetMatchDetail";
    public static string PL_GetMatchOfficialCommand => "PL_GetMatchOfficial";
}
