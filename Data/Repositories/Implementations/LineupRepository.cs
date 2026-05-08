using System.Data.SqlClient;
using System.Text.Json;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Helper;
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

    public async Task<bool> AddLineupAsync(LineupDto lineupDto)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = AddMatchLineupCommand;
            cmd.Parameters.AddWithValue("@MatchId", lineupDto.MatchId);
            cmd.Parameters.AddWithValue("@HomeClubFormationId", lineupDto.HomeClubFormationId);
            cmd.Parameters.AddWithValue("@AwayClubFormationId", lineupDto.AwayClubFormationId);
            cmd.Parameters.AddWithValue("@HomeClubLineupJson", JsonSerializer.Serialize(lineupDto.HomeClubLineup));
            cmd.Parameters.AddWithValue("@AwayClubLineupJson", JsonSerializer.Serialize(lineupDto.AwayClubLineup));
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteLineupAsync(int matchId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = DeleteMatchLineupCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<LineupDetailDto>> GetAllLineupsAsync(int? page = 1, int? pageSize = 20, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAllLineupsCommand;
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var lineupDetail = new List<LineupDetailDto>();
            if (rdr is not null)
            {
                do
                {
                    lineupDetail.Add(new LineupDetailDto
                    {
                        MatchId = rdr.SafeGetInt("MatchId"),
                        MatchDate = rdr.SafeGetString("MatchDate"),
                        KickoffTime = rdr.SafeGetString("KickoffTime"),
                        HomeClubId = rdr.SafeGetInt("HomeClubId"),
                        AwayClubId = rdr.SafeGetInt("AwayClubId"),
                        HomeClubName = rdr.SafeGetString("HomeClubName"),
                        AwayClubName = rdr.SafeGetString("AwayClubName"),
                        HomeClubCrest = rdr.SafeGetString("HomeClubCrest"),
                        AwayClubCrest = rdr.SafeGetString("AwayClubCrest"),
                        HomeClubTheme = rdr.SafeGetString("HomeClubTheme"),
                        AwayClubTheme = rdr.SafeGetString("AwayClubTheme"),
                        HomeClubGoal = rdr.SafeGetInt("HomeClubGoal"),
                        AwayClubGoal = rdr.SafeGetInt("AwayClubGoal"),
                        KickoffStadium = rdr.SafeGetString("KickoffStadium"),
                        IsGamePlaying = rdr.SafeGetBoolean("IsGamePlaying"),
                        MatchStatus = rdr.SafeGetString("MatchStatus")
                    });
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return lineupDetail;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database fetching lineup detail failed: {ex.Message}", ex);
        }
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

    public async Task<LineupClubInfoDto> GetLineupClubInfoByMatchIdAsync(int matchId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetLineupClubInfoByMatchIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return new LineupClubInfoDto
                {
                    MatchId = rdr.GetInt32(rdr.GetOrdinal("MatchId")),
                    HomeClubId = rdr.GetInt32(rdr.GetOrdinal("HomeClubId")),
                    AwayClubId = rdr.GetInt32(rdr.GetOrdinal("AwayClubId")),
                    HomeClubName = rdr.GetString(rdr.GetOrdinal("HomeClubName")),
                    AwayClubName = rdr.GetString(rdr.GetOrdinal("AwayClubName")),
                    HomeClubManager = rdr.GetString(rdr.GetOrdinal("HomeClubManager")),
                    AwayClubManager = rdr.GetString(rdr.GetOrdinal("AwayClubManager")),
                    HomeClubCrest = rdr.GetString(rdr.GetOrdinal("HomeClubCrest")),
                    AwayClubCrest = rdr.GetString(rdr.GetOrdinal("AwayClubCrest")),
                    HomeClubTheme = rdr.GetString(rdr.GetOrdinal("HomeClubTheme")),
                    AwayClubTheme = rdr.GetString(rdr.GetOrdinal("AwayClubTheme")),
                    HomeClubFormationId = rdr.GetInt32(rdr.GetOrdinal("HomeClubFormationId")),
                    AwayClubFormationId = rdr.GetInt32(rdr.GetOrdinal("AwayClubFormationId")),
                    HomeClubFormation = rdr.GetString(rdr.GetOrdinal("HomeClubFormation")),
                    AwayClubFormation = rdr.GetString(rdr.GetOrdinal("AwayClubFormation")),
                };
            }
            return new LineupClubInfoDto();
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<LineupFormationDetailDto>> GetLineupFormationByMatchIdAsync(int matchId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetLineupFormationDetailByMatchIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var lineupFormationDetail = new List<LineupFormationDetailDto>();
            if (rdr is not null)
            {
                do
                {
                    lineupFormationDetail.Add(new LineupFormationDetailDto
                    {
                        MatchId = rdr.SafeGetInt("MatchId"),
                        ClubId = rdr.SafeGetInt("ClubId"),
                        IsHomeClub = rdr.SafeGetBoolean("IsHomeClub"),
                        PlayerId = rdr.SafeGetInt("PlayerId"),
                        PlayerShortName = rdr.SafeGetString("PlayerShortName"),
                        PlayerNumber = rdr.SafeGetString("PlayerNumber"),
                        Position = rdr.SafeGetString("Position"),
                        Formation = rdr.SafeGetString("Formation"),
                        PlayerPhoto = rdr.SafeGetString("PlayerPhoto"),
                        ClubTheme = rdr.SafeGetString("ClubTheme"),
                        FormationSlot = rdr.SafeGetInt("FormationSlot"),
                        IsStarting = rdr.SafeGetBoolean("IsStarting")
                    });
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return lineupFormationDetail;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database fetching lineup formation detail failed: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<LineupFormationLayout>> GetLineupFormationLayoutByFormationIdAsync(int formationId = 1, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetLineupFormationLayoutByFormationIdCommand;
            cmd.Parameters.AddWithValue("@FormationId", formationId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var lineupFormationLayout = new List<LineupFormationLayout>();
            if (rdr is not null)
            {
                do
                {
                    lineupFormationLayout.Add(new LineupFormationLayout
                    {
                        Row = rdr.SafeGetInt("Row"),
                        PositionId = rdr.SafeGetInt("PositionId"),
                        Code = rdr.SafeGetString("Code"),
                        FormationName = rdr.SafeGetString("FormationName")
                    });
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return lineupFormationLayout;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database fetching lineup formation layout failed: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<LineupFormationDetailDto>> GetSubstitutionFormationByMatchIdAsync(int matchId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetSubstitutionFormationByMatchIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var lineupFormationDetail = new List<LineupFormationDetailDto>();
            if (rdr is not null)
            {
                do
                {
                    lineupFormationDetail.Add(new LineupFormationDetailDto
                    {
                        MatchId = rdr.SafeGetInt("MatchId"),
                        ClubId = rdr.SafeGetInt("ClubId"),
                        IsHomeClub = rdr.SafeGetBoolean("IsHomeClub"),

                        PlayerId = rdr.SafeGetInt("PlayerId"),
                        PlayerShortName = rdr.SafeGetString("PlayerShortName"),
                        PlayerNumber = rdr.SafeGetString("PlayerNumber"),
                        Position = rdr.SafeGetString("Position"),
                        Formation = rdr.SafeGetString("Formation"),
                        PlayerPhoto = rdr.SafeGetString("PlayerPhoto"),
                        ClubTheme = rdr.SafeGetString("ClubTheme"),
                        FormationSlot = rdr.SafeGetInt("FormationSlot"),
                        IsStarting = rdr.SafeGetBoolean("IsStarting")
                    });
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return lineupFormationDetail;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database fetching substitution formation failed: {ex.Message}", ex);
        }
    }

    public async Task<bool> UpdateLineupAsync(int matchId, LineupDto lineupDto)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = UpdateMatchLineupCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            cmd.Parameters.AddWithValue("@HomeClubFormationId", lineupDto.HomeClubFormationId);
            cmd.Parameters.AddWithValue("@AwayClubFormationId", lineupDto.AwayClubFormationId);
            cmd.Parameters.AddWithValue("@HomeClubLineupJson", JsonSerializer.Serialize(lineupDto.HomeClubLineup));
            cmd.Parameters.AddWithValue("@AwayClubLineupJson", JsonSerializer.Serialize(lineupDto.AwayClubLineup));
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> IsExistLineupAsync(int matchId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = IsExistLineupCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            return await execute.ExecuteScalarAsync<bool>(cmd);
        }
        catch (SqlException ex)
        {
            throw new Exception("Database IsExistLineup error: " + ex.Message);
        }
    }

    public async Task<LineupDto> GetLineupByMatchIdAsync(int matchId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetLineupByMatchIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            if (rdr is not null)
            {
                return new LineupDto
                {
                    MatchId = rdr.GetInt32(rdr.GetOrdinal("MatchId")),
                    HomeClubFormationId = rdr.GetInt32(rdr.GetOrdinal("HomeClubFormationId")),
                    AwayClubFormationId = rdr.GetInt32(rdr.GetOrdinal("AwayClubFormationId")),
                    HomeClubLineup = JsonSerializer.Deserialize<List<LineupClubDto>>(rdr.GetString(rdr.GetOrdinal("HomeClubLineup")))!,
                    AwayClubLineup = JsonSerializer.Deserialize<List<LineupClubDto>>(rdr.GetString(rdr.GetOrdinal("AwayClubLineup")))!
                };
            }
            return new LineupDto();
        }
        catch (SqlException ex)
        {
            throw new Exception("Database GetLineupByMatchId error: " + ex.Message);
        }
    }

    public async Task<bool> IsMatchCurringKickOffAsync(int matchId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = IsMatchCurringKickOffCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            return await execute.ExecuteScalarAsync<bool>(cmd) ? true : false;
        }
        catch (SqlException ex)
        {
            throw new Exception("Database IsMatchCurringKickOff error: " + ex.Message);
        }
    }

    public async Task<bool> IsMatchEndedAsync(int matchId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = IsMatchEndedCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            return await execute.ExecuteScalarAsync<bool>(cmd) ? true : false;
        }
        catch (SqlException ex)
        {
            throw new Exception("Database IsMatchEnded error: " + ex.Message);
        }
    }
}
