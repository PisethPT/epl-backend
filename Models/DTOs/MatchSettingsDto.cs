namespace epl_backend.Models.DTOs;

public class MatchSettingsDto
{
    public int MatchId { get; set; }
    public int HomeClubScores { get; set; }
    public int AwayClubScores { get; set; }
    public int KickoffStatus { get; set; }
    public bool IsGameFinish { get; set; }
}
