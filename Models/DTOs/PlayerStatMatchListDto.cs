namespace PremierLeague_Backend.Models.DTOs;

public record class PlayerStatMatchListDto
(
    int MatchId,
    string MatchDate,
    string MatchTime,
    string SeasonName,
    int MatchWeek,
    int HomeClubId,
    string HomeClubName,
    string HomeClubCrest,
    string HomeClubTheme,
    int HomeClubGoal,
    int AwayClubId,
    string AwayClubName,
    string AwayClubCrest,
    string AwayClubTheme,
    int AwayClubGoal,
    string KickoffStadium,
    bool IsGamePlaying,
    string MatchStatus
);