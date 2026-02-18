namespace PremierLeague_Backend.Models.DTOs;

public record PlayerStatGetStatsByCategoryDto
(
    int StatId,
    string StatName,
    string Symbol,
    bool IsPlayerStat,
    bool IsClubStat
);