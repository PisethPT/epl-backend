using System.Data.SqlClient;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Services.Interfaces;
namespace PremierLeague_Backend.Data.Repositories.Implementations;

using static PremierLeague_Backend.Helper.SqlCommands.LineupCommands;

public class LineupRepository : ILineupRepository
{
    private readonly IExecute execute;

    public LineupRepository(IExecute execute)
    {
        this.execute = execute;
    }
    public Task<bool> CreateLineupAsync(LineupDto lineupDto)
    {
        throw new NotImplementedException();
    }

    public async Task<(int, int)> GetHomeClubAndAwayClubByMatchIdAsync(int matchId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetHomeClubAndAwayClubByMatchIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return (rdr.GetInt32(rdr.GetOrdinal("HomeClubId")), rdr.GetInt32(rdr.GetOrdinal("AwayClubId")));
            }
            return (0, 0);
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
