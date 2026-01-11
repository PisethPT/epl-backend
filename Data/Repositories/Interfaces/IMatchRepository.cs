using System;
using epl_backend.Models.DTOs;

namespace epl_backend.Data.Repositories.Interfaces;

public interface IMatchRepository
{
    Task<bool> AddMatchAsync(MatchDto matchDto);
    Task<bool> UpdateMatchAsync(MatchDto matchDto);
    Task<bool> DeleteMatchAsync(int matchId);
    Task<List<MatchDetailDto>> GetAllMatchAsync(int? seasonId, int week, int? page = 1, string? seasonJson = null, string? matchWeekJson = null, string? clubIdJson = null, CancellationToken ct = default);
    Task<MatchSeasonDto> GetMatchWeekAsync();
    Task<MatchDto> FindMatchByIdAsync(int matchId);
    Task<bool> FindClubCannotBeTheSameAsync(int homeClubId, int awayClubId);
    Task<bool> FindMatchDateIsOutsideTheActiveSeason(DateOnly matchDate, DateOnly startSeasonDate, DateOnly endSeasonDate);
    Task<bool> FindMatchDateCannotBeEarlierThanToday(DateOnly matchDate);
    Task<bool> FindMatchAlreadyExistsOnTheSelectedDate(DateOnly matchDate, int homeClubId, int awayClubId);
    Task<int> CountMatchesAsync(string? seasonJson = null, string? matchWeekJson = null, string? clubIdJson = null);
    Task<MatchWeekDto> GetMatchWeekBySeasonIdAsync(int seasonId, int week, CancellationToken ct = default);
}
