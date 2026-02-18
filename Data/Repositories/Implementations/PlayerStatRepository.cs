using System.Data.SqlClient;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Helper;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Services.Interfaces;
using static PremierLeague_Backend.Helper.SqlCommands.PlayerStatCommand;

namespace PremierLeague_Backend.Data.Repositories.Implementations;

public class PlayerStatRepository : IPlayerStatRepository
{
    private readonly IExecute execute;

    public PlayerStatRepository(IExecute execute)
    {
        this.execute = execute;
    }
    public Task<bool> AddPlayerStatAsync(PlayerStatDto playerStatDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePlayerStatAsync(int playerStatId)
    {
        throw new NotImplementedException();
    }

    public Task<PlayerStatDto> FindPlayerStatByIdAsync(int playerStatId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PlayerStatGetPlayersDto>> GetAwayPlayersForPlayerStatByClubIdAndMatchIdAsync(int matchId, int clubId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAwayPlayersForPlayerStatByClubIdAndMatchIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            cmd.Parameters.AddWithValue("@ClubId", clubId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var players = new List<PlayerStatGetPlayersDto>();
            if (rdr is not null)
            {
                do
                {
                    players.Add(new PlayerStatGetPlayersDto(
                        MatchId: rdr.SafeGetInt("MatchId"),
                        ClubId: rdr.SafeGetInt("ClubId"),
                        PlayerId: rdr.SafeGetInt("PlayerId"),
                        ClubName: rdr.SafeGetString("ClubName"),
                        ClubCrest: rdr.SafeGetString("ClubCrest"),
                        ClubTheme: rdr.SafeGetString("ClubTheme"),
                        FirstName: rdr.SafeGetString("FirstName"),
                        LastName: rdr.SafeGetString("LastName"),
                        Position: rdr.SafeGetString("Position"),
                        PlayerNumber: rdr.SafeGetInt("PlayerNumber"),
                        Photo: rdr.SafeGetString("Photo")
                    ));
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return players;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<PlayerStatGetPlayersDto>> GetHomePlayersForPlayerStatByClubIdAndMatchIdAsync(int matchId, int clubId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetHomePlayersForPlayerStatByClubIdAndMatchIdCommand;
            cmd.Parameters.AddWithValue("@MatchId", matchId);
            cmd.Parameters.AddWithValue("@ClubId", clubId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var players = new List<PlayerStatGetPlayersDto>();
            if (rdr is not null)
            {
                do
                {
                    players.Add(new PlayerStatGetPlayersDto(
                        MatchId: rdr.SafeGetInt("MatchId"),
                        ClubId: rdr.SafeGetInt("ClubId"),
                        PlayerId: rdr.SafeGetInt("PlayerId"),
                        ClubName: rdr.SafeGetString("ClubName"),
                        ClubCrest: rdr.SafeGetString("ClubCrest"),
                        ClubTheme: rdr.SafeGetString("ClubTheme"),
                        FirstName: rdr.SafeGetString("FirstName"),
                        LastName: rdr.SafeGetString("LastName"),
                        Position: rdr.SafeGetString("Position"),
                        PlayerNumber: rdr.SafeGetInt("PlayerNumber"),
                        Photo: rdr.SafeGetString("Photo")
                    ));
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return players;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<PlayerStatDetailDto> GetPlayerStatDetailDtoAsync(int matchId, int clubId, int playerId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PlayerStatMatchListDto>> GetPlayerStatMatchListAsync(int? seasonId, int week, int? page = 1, int? competitionId = 1, string? clubIdJson = null, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetPlayerStatMatchListCommand;
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@SeasonId", seasonId);
            cmd.Parameters.AddWithValue("@Week", week);
            cmd.Parameters.AddWithValue("@CompetitionId", competitionId);
            cmd.Parameters.AddWithValue("@ClubIdJson", clubIdJson ?? (object)DBNull.Value);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var playerStatMatchList = new List<PlayerStatMatchListDto>();
            if (rdr is not null)
            {
                do
                {
                    playerStatMatchList.Add(new PlayerStatMatchListDto
                    (
                        MatchId: rdr.SafeGetInt("MatchId"),
                        MatchDate: rdr.SafeGetString("MatchDate"),
                        MatchTime: rdr.SafeGetString("MatchTime"),
                        SeasonName: rdr.SafeGetString("SeasonName"),
                        MatchWeek: rdr.SafeGetInt("HomeClubId"),
                        HomeClubId: rdr.SafeGetInt("HomeClubId"),
                        HomeClubName: rdr.SafeGetString("HomeClubName"),
                        HomeClubCrest: rdr.SafeGetString("HomeClubCrest"),
                        HomeClubTheme: rdr.SafeGetString("HomeClubTheme"),
                        HomeClubGoal: rdr.SafeGetInt("HomeClubGoal"),
                        AwayClubId: rdr.SafeGetInt("AwayClubId"),
                        AwayClubName: rdr.SafeGetString("AwayClubName"),
                        AwayClubCrest: rdr.SafeGetString("AwayClubCrest"),
                        AwayClubTheme: rdr.SafeGetString("AwayClubTheme"),
                        AwayClubGoal: rdr.SafeGetInt("AwayClubGoal"),
                        KickoffStadium: rdr.SafeGetString("KickoffStadium"),
                        IsGamePlaying: rdr.SafeGetBoolean("IsGamePlaying"),
                        MatchStatus: rdr.SafeGetString("MatchStatus")
                    ));
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return playerStatMatchList;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<PlayerStatGetStatsByCategoryDto>> GetStatByCategoryIdForPlayerStatAsync(int statCategoryId, int statScopeId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAwayPlayersForPlayerStatByClubIdAndMatchIdCommand;
            cmd.Parameters.AddWithValue("@StatCategoryId", statCategoryId);
            cmd.Parameters.AddWithValue("@StatScopeId", statScopeId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var players = new List<PlayerStatGetStatsByCategoryDto>();
            if (rdr is not null)
            {
                do
                {
                    players.Add(new PlayerStatGetStatsByCategoryDto(
                        StatId: rdr.SafeGetInt("StatId"),
                        StatName: rdr.SafeGetString("StatName"),
                        Symbol: rdr.SafeGetString("Symbol"),
                        IsPlayerStat: rdr.SafeGetBoolean("IsPlayerStat"),
                        IsClubStat: rdr.SafeGetBoolean("IsClubStat")
                    ));
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return players;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<bool> UpdatePlayerStatAsync(int playerStatId, PlayerStatDto playerStatDto)
    {
        throw new NotImplementedException();
    }
}
