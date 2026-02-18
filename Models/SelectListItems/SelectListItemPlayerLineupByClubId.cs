namespace PremierLeague_Backend.Models.SelectListItems;

public record SelectListItemPlayerLineupByClubId(
    int ClubId,
    string ClubCrest,
    string ClubTheme,
    int PlayerId,
    string FirstName,
    string LastName,
    string Photo,
    int PlayerNumber,
    string PreferredFoot,
    int PositionId,
    string Position
);