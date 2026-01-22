namespace epl_backend.Models.DTOs;

public class LineupDto
{
    public int MatchId { get; set; }
    public List<LineupClubDto> HomeClubLineup { get; set; } = new();
    public List<LineupClubDto> AwayClubLineup { get; set; } = new();
}
