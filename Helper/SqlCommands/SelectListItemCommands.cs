namespace PremierLeague_Backend.Helper.SqlCommands;

public static class SelectListItemCommands
{
    public static string SelectListItemClubCommands => "PL_SelectListItemClub";
    public static string SelectListItemSeasonCommands => "PL_SelectListItemSeason";
    public static string SelectListItemPlayerLineupByClubIdCommands => "PL_SelectListItemPlayerLineupByClubId";
    public static string SelectListItemRefereeCommands => "PL_SelectListItemReferee";
    public static string SelectListItemRefereeRoleCommands => "PL_SelectListItemRefereeRole";
    public static string SelectListItemRefereeBadgeLevelCommands => "PL_SelectListItemRefereeBadgeLevel";
    public static string SelectListItemMatchForLineupCommands => "PL_SelectListItemMatchForLineup";
    public static string SelectListItemFormationCommands => "PL_SelectListItemFormation";
    
    // Match Select List Items
    public static string SelectListItemMatchWeekCommandText => "PL_CommandSelectListItemMatchWeek";
    public static string SelectListItemSeasonCommandText => "PL_CommandSelectListItemSeason";

    // Stats Select List Items
    public static string CommandSelectListItemStatsCommandText => "PL_CommandSelectListItemStats";
    public static string CommandSelectListItemStatCategoriesForMatchKickoffCommandText => "PL_CommandSelectListItemStatCategoriesForMatchKickoff";
    public static string CommandSelectListItemStatScopeCommandText => "PL_CommandSelectListItemStatScope";
}
