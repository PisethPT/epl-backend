using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Models.DTOs;
using epl_backend.Services.Interfaces;

namespace epl_backend.Data.Repositories.Implementations;

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
}
