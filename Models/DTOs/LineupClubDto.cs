namespace PremierLeague_Backend.Models.DTOs;

public record LineupClubDto
(
    int ClubId,
    int PlayerId,
    int FormationPositionId,
    bool IsStarting,
    int FormationSlot
);
