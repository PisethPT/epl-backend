namespace PremierLeague_Backend.Models.DTOs;

public record GoalDetailDto
(
    int GoalId,
    int MatchId,
    int PlayerId,
    int ClubId,
    int Minute,
    string FirstName,
    string LastName,
    int PlayerNumber,
    string Position,
    string IsPlayerHomeClub,
    string MatchDate,
    string MatchTime,
    string SeasonName,
    int MatchWeek,
    string HomeClubName,
    string HomeClubCrest,
    string HomeClubTheme,
    int HomeClubGoal,
    string AwayClubName,
    string AwayClubCrest,
    string AwayClubTheme,
    int AwayClubGoal,
    string KickoffStatus,
    string IsGameFinish,
    string KickoffStadium
);
