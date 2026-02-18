using System.ComponentModel.DataAnnotations;

namespace PremierLeague_Backend.Models.DTOs;

public class UserLoginDto
{
    [Required(ErrorMessage = "Please input your email.")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please input your password.")]
    public string? Password { get; set; }
    public bool RememberMe { get; internal set; }
}
