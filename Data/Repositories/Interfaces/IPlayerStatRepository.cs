using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Data.Repositories.Interfaces;

public interface IPlayerStatRepository
{
    Task<bool> AddPlayerStatAsync(PlayerStatDto playerStatDto);
    Task<bool> UpdatePlayerStatAsync(int playerStatId, PlayerStatDto playerStatDto);
    Task<bool> DeletePlayerStatAsync(int playerStatId);
    Task<PlayerStatDto> FindPlayerStatByIdAsync(int playerStatId, CancellationToken ct = default);
    Task<IEnumerable<PlayerStatMatchListDto>> GetPlayerStatMatchListAsync(int? seasonId, int week, int? page = 1, int? competitionId = 1, string? clubIdJson = null, CancellationToken ct = default);
    Task<PlayerStatDetailDto> GetPlayerStatDetailDtoAsync(int matchId, int clubId, int playerId, CancellationToken ct = default);
    Task<IEnumerable<PlayerStatGetPlayersDto>> GetHomePlayersForPlayerStatByClubIdAndMatchIdAsync(int matchId, int clubId, CancellationToken ct = default);
    Task<IEnumerable<PlayerStatGetPlayersDto>> GetAwayPlayersForPlayerStatByClubIdAndMatchIdAsync(int matchId, int clubId, CancellationToken ct = default);
    Task<IEnumerable<PlayerStatGetStatsByCategoryDto>> GetStatByCategoryIdForPlayerStatAsync(int statCategoryId, int statScopeId, CancellationToken ct = default);
}
