namespace PremierLeague_Backend.Models.DTOs;

public class MatchWeekDto
{
    public int Week { get; set; }
    public int PrevWeek { get; set; }
    public int NextWeek { get; set; }
    public int SeasonId { get; set; }
    public string MatchWeek { get; set; } = string.Empty;
    public string MatchRangId { get; set; } = string.Empty;
    public string MatchWeekDate { get; set; } = string.Empty;
}
