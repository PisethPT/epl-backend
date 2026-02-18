using System.ComponentModel.DataAnnotations;

namespace PremierLeague_Backend.Models.DTOs;

public class MatchDto
{
    public int? MatchId { get; set; }

    [Required(ErrorMessage = "Match date is required.")]
    public DateOnly MatchDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [Required(ErrorMessage = "Match time is required.")]
    public TimeOnly MatchTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now);

    [Range(1, int.MaxValue, ErrorMessage = "Home club is required.")]
    public int HomeClubId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Away club is required.")]
    public int AwayClubId { get; set; }

    public bool IsHomeStadium { get; set; } = true;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Season is required.")]
    public int SeasonId { get; set; }

    [Required]
    public int? MatchWeek { get; set; }

    public List<MatchRefereeDto>? MatchReferees { get; set; } = new List<MatchRefereeDto>
    {
        new MatchRefereeDto { RoleId = 1},
        new MatchRefereeDto { RoleId = 2},
        new MatchRefereeDto { RoleId = 3},
        new MatchRefereeDto { RoleId = 4},
        new MatchRefereeDto { RoleId = 5}
    };
}
