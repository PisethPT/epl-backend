using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Data.Repositories.Interfaces;

public interface INewsRepository
{
    Task<bool> AddNewsAsync(NewsDto newsDto);
    Task<bool> UpdateNewsAsync(NewsDto newsDto);
    Task<bool> DeleteNewsAsync(int newsId, string? imageUrl);
    Task<NewsDto?> GetNewsByIdAsync(int newsId, CancellationToken ct = default);
     Task<bool> FindNewsExisting(NewsDto newsDto, int? newsId = 0);
    Task<IEnumerable<NewsDetailDto>> GetAllNewsDetailAsync(int page = 1, CancellationToken ct = default);
    Task<int> CountNewsAsync(CancellationToken ct = default);
    Task<IEnumerable<PlayerStatGetPlayersDto>> GetAllPlayersAsync(CancellationToken ct = default);
}
