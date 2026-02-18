using System.Data.SqlClient;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Services.Interfaces;
using static PremierLeague_Backend.Helper.SqlCommands.GoalCommands;

namespace PremierLeague_Backend.Data.Repositories.Implementations;

public class GoalRepository : IGoalRepository
{
    private readonly IExecute execute;

    public GoalRepository(IExecute execute)
    {
        this.execute = execute;
    }

    public Task<bool> AddGoalAsync(GoalDto goalDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteGoalAsync(int goalId)
    {
        throw new NotImplementedException();
    }

    public Task<GoalDto> FindGoalByIdAsync(int goalId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<GoalDetailDto>> GetAllGoalsAsync(int? seasonId, int week, int? page = 1, int? competitionId = 1, string? clubIdJson = null, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAllGoalsCommand;
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@SeasonId", seasonId);
            cmd.Parameters.AddWithValue("@Week", week);
            cmd.Parameters.AddWithValue("@CompetitionId", competitionId);
            cmd.Parameters.AddWithValue("@ClubIdJson", clubIdJson ?? (object)DBNull.Value);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var goalDetail = new List<GoalDetailDto>();
            if (rdr is not null)
            {
                do
                {
                    goalDetail.Add(new GoalDetailDto(
                        GoalId: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("GoalId"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("GoalId")),
                        MatchId: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("MatchId"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("MatchId")),
                        PlayerId: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("PlayerId"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("PlayerId")),
                        ClubId: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("ClubId"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("ClubId")),
                        Minute: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("Minute"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("Minute")),
                        FirstName: rdr.GetString(rdr.GetOrdinal("FirstName")),
                        LastName: rdr.GetString(rdr.GetOrdinal("LastName")),
                        PlayerNumber: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("PlayerNumber"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("PlayerNumber")),
                        Position: rdr.GetString(rdr.GetOrdinal("Position")),
                        IsPlayerHomeClub: rdr.GetString(rdr.GetOrdinal("IsPlayerHomeClub")),
                        MatchDate: rdr.GetString(rdr.GetOrdinal("MatchDate")),
                        MatchTime: rdr.GetString(rdr.GetOrdinal("MatchTime")),
                        SeasonName: rdr.GetString(rdr.GetOrdinal("SeasonName")),
                        MatchWeek: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("MatchWeek"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("MatchWeek")),
                        HomeClubName: rdr.GetString(rdr.GetOrdinal("HomeClubName")),
                        HomeClubCrest: rdr.GetString(rdr.GetOrdinal("HomeClubCrest")),
                        HomeClubTheme: rdr.GetString(rdr.GetOrdinal("HomeClubTheme")),
                        HomeClubGoal: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("HomeClubGoal"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("HomeClubGoal")),
                        AwayClubName: rdr.GetString(rdr.GetOrdinal("AwayClubName")),
                        AwayClubCrest: rdr.GetString(rdr.GetOrdinal("AwayClubCrest")),
                        AwayClubTheme: rdr.GetString(rdr.GetOrdinal("AwayClubTheme")),
                        AwayClubGoal: rdr.IsDBNull(rdr.GetInt32(rdr.GetOrdinal("AwayClubGoal"))) ? 0 : rdr.GetInt32(rdr.GetOrdinal("AwayClubGoal")),
                        KickoffStatus: rdr.GetString(rdr.GetOrdinal("KickoffStatus")),
                        IsGameFinish: rdr.GetString(rdr.GetOrdinal("IsGameFinish")),
                        KickoffStadium: rdr.GetString(rdr.GetOrdinal("KickoffStadium"))

                    ));
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return goalDetail;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<bool> UpdateGoalAsync(int goalId, GoalDto goalDto)
    {
        throw new NotImplementedException();
    }
}
