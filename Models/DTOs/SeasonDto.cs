using System.ComponentModel.DataAnnotations;

namespace epl_backend.Models.DTOs;

public class SeasonDto
{
    public int? SeasonId { get; set; }

    [Required(ErrorMessage = "Please input season name.")]
    [RegularExpression(@"^\S+(\s+\S+)*$", ErrorMessage = "Season name cannot be empty or only whitespace.")]
    public string SeasonName { get; set; }

    [Required(ErrorMessage = "Please input season start date.")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "Please input season end date.")]
    public DateOnly? EndDate { get; set; }
}
