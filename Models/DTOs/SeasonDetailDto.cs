namespace PremierLeague_Backend.Models.DTOs;

public class SeasonDetailDto
{
    public int SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
}
