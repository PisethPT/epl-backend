using System.ComponentModel.DataAnnotations;
using epl_backend.Models.Enums;

namespace epl_backend.Models.DTOs;

public class UserDto
{
    // [MaxLength(length: 450)]
    public string? UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please input your first name.")]
    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "First name cannot be empty or whitespace.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Please input your last name.")]
    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "Last name cannot be empty or whitespace.")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Please input your email."), MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; }
    [Required(ErrorMessage = "Please input your phone number.")]
    [Phone(ErrorMessage = "Please input a valid phone number.")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "Please select a gender.")]
    public Gender Gender { get; set; }
    public string? PasswordHash { get; set; }
    public bool LockoutEnabled { get; set; } = false;
    public DateTimeOffset? LockoutEnd { get; set; }
    public string? Photo { get; set; }
    public IFormFile? PhotoFile { get; set; }
}
