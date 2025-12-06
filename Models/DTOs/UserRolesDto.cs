using System;
using System.ComponentModel.DataAnnotations;

namespace epl_backend.Models.DTOs;

public class UserRolesDto
{
    [MaxLength(length: 450)]
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
