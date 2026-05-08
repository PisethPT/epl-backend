using System.ComponentModel.DataAnnotations;

namespace PremierLeague_Backend;

public class MatchInfoDto
{
    public int? MatchInfoId { get; set; }
    [Required(ErrorMessage = "Match is required.")]
    public int MatchId { get; set; }
    public int Attendance { get; set; }
    public string? Weather { get; set; }
    public string? PitchCondition { get; set; }
    public int? AddedTimeFirstHalf { get; set; }
    public int? AddedTimeSecondHalf { get; set; }
    [MaxLength(500)]
    [Required(ErrorMessage = "Match Report Title is required.")]
    public string MatchReportTitle { get; set; }
    [Required(ErrorMessage = "Match Report Content is required")]
    public string MatchReportContent { get; set; }
    public string? AuthorId { get; set; }
    [MaxLength(1000)]
    [Required(ErrorMessage = "Home Club ReportUrl")]
    public string HomeClubReportUrl { get; set; }
    [MaxLength(1000)]
    [Required(ErrorMessage = "Away Club ReportUrl is required.")]
    public string AwayClubReportUrl { get; set; }
    [Required(ErrorMessage = "PlayerId is required.")]
    public int PlayerId { get; set; }
    [MaxLength(50)]
    public string? AwardBy { get; set; }
    [MaxLength(255)]
    public string? Notes { get; set; }

    public bool IsHomeClub { get; set; } = false;
}