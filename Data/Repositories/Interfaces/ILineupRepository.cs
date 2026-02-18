using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Data.Repositories.Interfaces;

public interface ILineupRepository
{
    Task<bool> CreateLineupAsync(LineupDto lineupDto);
    Task<(int, int)> GetHomeClubAndAwayClubByMatchIdAsync(int matchId);
}
