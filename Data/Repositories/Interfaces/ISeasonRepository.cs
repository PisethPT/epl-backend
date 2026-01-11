using epl_backend.Models.DTOs;

namespace epl_backend.Data.Repositories.Interfaces;

public interface ISeasonRepository
{
    Task<bool> AddSeasonAsync(SeasonDto seasonDto, CancellationToken ct = default);
    Task<bool> UpdateSeasonAsync(SeasonDto seasonDto,CancellationToken ct = default);
    Task<bool> DeleteSeasonAsync(int seasonId, CancellationToken ct = default);
    Task<List<SeasonDetailDto>> GetAllSeasonAsync(int? page = 1, CancellationToken ct = default);
    Task<SeasonDto> FindSeasonByIdAsync(int seasonId, CancellationToken ct = default);
    Task<bool> SeasonExistingAsync(SeasonDto seasonDto, int? seasonId = 0, CancellationToken ct = default);
    Task<int> CountSeasonsAsync(CancellationToken ct = default);
}
