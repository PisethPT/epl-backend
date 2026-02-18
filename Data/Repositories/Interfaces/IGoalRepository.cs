using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Data.Repositories.Interfaces;

public interface IGoalRepository
{
    Task<bool> AddGoalAsync(GoalDto goalDto);
    Task<bool> UpdateGoalAsync(int goalId, GoalDto goalDto);
    Task<bool> DeleteGoalAsync(int goalId);
    Task<GoalDto> FindGoalByIdAsync(int goalId, CancellationToken ct = default);
    Task<List<GoalDetailDto>> GetAllGoalsAsync(int? seasonId, int week, int? page = 1, int? competitionId = 1, string? clubIdJson = null, CancellationToken ct = default);
}
