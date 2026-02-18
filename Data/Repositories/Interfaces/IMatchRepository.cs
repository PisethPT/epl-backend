using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Data.Repositories.Interfaces;

public interface IMatchRepository
{
    Task<bool> AddMatchAsync(MatchDto matchDto);
    Task<bool> UpdateMatchAsync(MatchDto matchDto);
    Task<bool> DeleteMatchAsync(int matchId);
    Task<List<MatchDetailDto>> GetAllMatchAsync(int? seasonId, int week, int? page = 1, int? competitionId = 1, string? clubIdJson = null, CancellationToken ct = default);
    Task<MatchSeasonDto> GetMatchWeekAsync();
    Task<MatchDto> FindMatchByIdAsync(int matchId);
    Task<bool> FindClubCannotBeTheSameAsync(int homeClubId, int awayClubId);
    Task<bool> FindMatchDateIsOutsideTheActiveSeason(DateOnly matchDate, DateOnly startSeasonDate, DateOnly endSeasonDate);
    Task<bool> FindMatchDateCannotBeEarlierThanToday(DateOnly matchDate);
    Task<bool> FindMatchAlreadyExistsOnTheSelectedDate(int? matchId, DateOnly matchDate, int homeClubId, int awayClubId);
    Task<int> CountMatchesAsync(string? seasonJson = null, string? matchWeekJson = null, string? clubIdJson = null);
    Task<MatchWeekDto> GetMatchWeekBySeasonIdAsync(int seasonId, int week, CancellationToken ct = default);
    Task<List<MatchRefereeDto>> FindMatchRefereeByMatchIdAsync(int matchId, CancellationToken ct = default);
    Task<(int matchWeek, int seasonId)> GetCurrentMatchWeekAndSeasonIdAsync(CancellationToken ct = default);
    Task<MatchInfoMatchDetailsDto?> GetMatchInfoMatchDetailsByMatchIdAsync(int matchId, CancellationToken ct = default);
    Task<List<MatchInfoMatchOfficialsDto>> GetMatchInfoMatchOfficialsByMatchIdAsync(int matchId, CancellationToken ct = default);
}
