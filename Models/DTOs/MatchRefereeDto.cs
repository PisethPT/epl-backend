using System.ComponentModel.DataAnnotations;

namespace PremierLeague_Backend.Models.DTOs;

public class MatchRefereeDto
{
    public int? MatchRefereeId { get; set; }
    public int? MatchId { get; set; }

    [Required(ErrorMessage = "Referee is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Referee is required")]
    public int RefereeId { get; set; }

    public int RoleId { get; set; }
}
