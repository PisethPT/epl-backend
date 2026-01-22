namespace epl_backend.Models.SelectListItems;

public record SelectListItemMatchForLineup
(
    int MatchId,
    int HomeClubId,
    int AwayClubId,
    string HomeClubCrest,
    string AwayClubCrest,
    string HomeTheme,
    string AwayTheme,
    string MatchContent,
    string MatchSubContent
);
