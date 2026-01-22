using epl_backend.Models.DTOs;

namespace epl_backend.Data.Repositories.Interfaces;

public interface ILineupRepository
{
    Task<bool> CreateLineupAsync(LineupDto lineupDto);
}
