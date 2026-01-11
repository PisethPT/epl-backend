using epl_backend.Models.DTOs;

namespace epl_backend.Data.Repositories.Interfaces;

public interface IPlayerRepository
{
    Task<bool> AddPlayerAsync(PlayerDto playerDto, CancellationToken ct = default);
    Task<bool> UpdatePlayerAsync(PlayerDto playerDto, CancellationToken ct = default);
    Task<bool> DeletePlayerAsync(int playerId, string? photo, CancellationToken ct = default);
    Task<List<PlayerDetailDto>> GetAllPlayerAsync(string? positionJson = null, string? clubIdJson = null, int? page = 1, CancellationToken ct = default);
    Task<PlayerDto> FindPlayerByIdAsync(int playerId, CancellationToken ct = default);
    Task<bool> PlayerExistingByClubAsync(PlayerDto playerDto, int? playerId = 0, CancellationToken ct = default);
    Task<int> CountPlayersAsync(string? positionJson = null, string? clubIdJson = null, CancellationToken ct = default);
}
