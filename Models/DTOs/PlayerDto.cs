using System.ComponentModel.DataAnnotations;
using PremierLeague_Backend.Models.Enums;

namespace PremierLeague_Backend.Models.DTOs;

public class PlayerDto
{
    public int? PlayerId { get; set; }

    [Required(ErrorMessage = "Please input player's first name.")]
    [MaxLength(100)]
    [RegularExpression(@"^\S+(\s+\S+)*$", ErrorMessage = "First name cannot be empty or only whitespace.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Please input player's last name.")]
    [MaxLength(100)]
    [RegularExpression(@"^\S+(\s+\S+)*$", ErrorMessage = "Last name cannot be empty or only whitespace.")]
    public string? LastName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? PlaceOfBirth { get; set; }

    public string? Nationality { get; set; }

    public PreferredFoot? PreferredFoot { get; set; }

    // Height as text (e.g., "180 cm")
    [RegularExpression(@"^\S+(\s+\S+)*$", ErrorMessage = "Player height cannot be empty or only whitespace.")]
    public string? Height { get; set; }

    [Required(ErrorMessage = "Please select a playing position.")]
    public Position Position { get; set; }

    //[Required(ErrorMessage = "Please input player number.")]
    [Range(1, 99, ErrorMessage = "Player number must be between 1 and 99.")]
    public int? PlayerNumber { get; set; }

    //[Required(AllowEmptyStrings = false, ErrorMessage = "Please select player's joined club.")]
    public string? JoinedClub { get; set; }

    public string? Photo { get; set; }

    // [Required(ErrorMessage = "Please upload a player's photo.")]
    public IFormFile? PhotoFile { get; set; }

    public int ClubId { get; set; }
}
