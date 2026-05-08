namespace PremierLeague_Backend;
public class LineupFormationDetailDto
{
    public int MatchId { get; set; }
    public int ClubId { get; set; }
    public bool IsHomeClub { get; set; }
    public int PlayerId { get; set; }
    public string PlayerShortName { get; set; } = string.Empty;
    public string PlayerNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Formation { get; set; } = string.Empty;
    public string PlayerPhoto { get; set; } = string.Empty;
    public string ClubTheme { get; set; } = string.Empty;
    public int FormationSlot { get; set; }
    public bool IsStarting { get; set; }
}