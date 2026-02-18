using System.ComponentModel.DataAnnotations;

namespace PremierLeague_Backend.Models.DTOs;

public class UserRolesDto
{
    [MaxLength(length: 450)]
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
