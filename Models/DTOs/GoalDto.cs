namespace PremierLeague_Backend.Models.DTOs;

public record GoalDto(int MatchId, int? GoalId, int PlayerId, int ClubId, int Minute);
