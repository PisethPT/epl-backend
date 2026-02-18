namespace PremierLeague_Backend.Models.DTOs;

public class MatchDetailDto
{
    public int MatchId { get; set; }
    public string MatchDate { get; set; } = string.Empty;
    public string MatchTime { get; set; } = string.Empty;
    public string SeasonName { get; set; } = string.Empty;
    public int MatchWeek { get; set; }
    public int HomeClubId { get; set; }
    public int AwayClubId { get; set; }
    public string HomeClubName { get; set; } = string.Empty;
    public string AwayClubName { get; set; } = string.Empty;
    public string HomeClubCrest { get; set; } = string.Empty;
    public string AwayClubCrest { get; set; } = string.Empty;
    public string HomeClubTheme { get; set; } = string.Empty;
    public string AwayClubTheme { get; set; } = string.Empty;
    public int HomeClubScore { get; set; }
    public int AwayClubScore { get; set; }
    public string KickoffStatus { get; set; } = string.Empty;
    public string IsGameFinish { get; set; } = string.Empty;
    public string KickoffStadium { get; set; } = string.Empty;
}
