namespace PremierLeague_Backend.Helper.SqlCommands;

public static class GoalCommands
{
    public static string GetAllGoalsCommand => "PL_GetAllGoals";
    public static string AddGoalCommand => "PL_AddGoal";
    public static string UpdateGoalCommand => "PL_UpdateGoal";
    public static string DeleteGoalCommand => "PL_DeleteGoal";
    public static string FindGoalAlreadyExistsOnTheSameMinuteWithinPlayerIdCommand => "PL_FindGoalAlreadyExistsOnTheSameMinuteWithinPlayerId";
}
