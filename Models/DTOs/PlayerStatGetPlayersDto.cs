namespace PremierLeague_Backend.Models.DTOs;

public record PlayerStatGetPlayersDto
(
    int MatchId,
    int ClubId,
    int PlayerId,
    string ClubName,
    string ClubCrest,
    string ClubTheme,
    string? FirstName,
    string? LastName,
    string? Position,
    int? PlayerNumber,
    string? Photo
);
