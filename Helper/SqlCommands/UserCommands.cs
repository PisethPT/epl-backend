namespace PremierLeague_Backend.Helper.SqlCommands;

public static class UserCommands
{
    public static string CheckAspNetUserPasswordCommand => "PL_CheckUserPassword";
    public static string FindAspNetUserByEmailCommand => "PL_FindUserByEmail";
    public static string FindAspNetUserByUserIdCommand => "PL_FindUserByUserId";
    public static string UserByExistEmailCommand => "PL_UserByExistEmail";
    public static string GetAllAspNetUsersCommand => "PL_GetAllUsers";
    public static string GetAspNetUserRolesCommand => "PL_GetUserRoles";
    public static string AddAspNetUserCommand => "PL_AddAspNetUser";
    public static string UpdateAspNetUserCommand => "PL_UpdateAspNetUser";
    public static string DeleteAspNetUserCommand => "PL_DeleteAspNetUser";
}