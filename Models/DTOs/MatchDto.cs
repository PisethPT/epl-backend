using System.ComponentModel.DataAnnotations;

namespace epl_backend.Models.DTOs;

public class MatchDto : IValidatableObject
{
    public int? MatchId { get; set; }

    [Required(ErrorMessage = "Match date is required.")]
    public DateOnly MatchDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [Required(ErrorMessage = "Match time is required.")]
    public TimeOnly MatchTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now);

    [Required(ErrorMessage = "Home club is required.")]
    public int HomeClubId { get; set; }

    [Required(ErrorMessage = "Away club is required.")]
    public int AwayClubId { get; set; }

    public bool IsHomeStadium { get; set; }

    public int? SeasonId { get; set; }

    public int? MatchWeek { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (HomeClubId == AwayClubId)
        {
            yield return new ValidationResult(
                "Home club and Away club must be different.",
                new[] { nameof(HomeClubId), nameof(AwayClubId) }
            );
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        if (MatchDate < today)
        {
            yield return new ValidationResult(
                "Match date cannot be in the past.",
                new[] { nameof(MatchDate) }
            );
            yield break;
        }

        if (MatchDate == today)
        {
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);

            if (MatchTime <= nowTime)
            {
                yield return new ValidationResult(
                    "Match time must be in the future.",
                    new[] { nameof(MatchTime) }
                );
            }
        }
    }
}
