using System.ComponentModel.DataAnnotations;
using epl_backend.Models.Enums;

namespace epl_backend.Models.DTOs;

public class PlayerDto
{
    public int? PlayerId { get; set; }

    [Required(ErrorMessage = "Please input player's first name.")]
    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "First name cannot be only whitespace.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Please input player's last name.")]
    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "Last name cannot be only whitespace.")]
    public string LastName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? PlaceOfBirth { get; set; }

    public string? Nationality { get; set; }

    public PreferredFoot? PreferredFoot { get; set; }

    // Height as text (e.g., "180 cm")
    // [RegularExpression(@"\S+", ErrorMessage = "Player height cannot be empty or whitespace.")]
    public string? Height { get; set; }

    [Required(ErrorMessage = "Please select a playing position.")]
    public Position Position { get; set; }

    [Required(ErrorMessage = "Please input player number.")]
    [Range(1, 99, ErrorMessage = "Player number must be between 1 and 99.")]
    public int? PlayerNumber { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Please select player's joined club.")]
    public string? JoinedClub { get; set; }

    public string? Photo { get; set; }

    // [Required(ErrorMessage = "Please upload a player's photo.")]
    public IFormFile? PhotoFile { get; set; }

    public int ClubId { get; set; }
}
