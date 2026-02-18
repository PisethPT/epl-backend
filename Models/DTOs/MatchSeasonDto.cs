namespace PremierLeague_Backend.Models.DTOs;

public class MatchSeasonDto
{
    public int SeasonId { get; set; }
    public int MatchWeek { get; set; }
    public DateOnly StartSeasonDate { get; set; }
    public DateOnly EndSeasonDate { get; set; }
}
