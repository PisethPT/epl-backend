using System;

namespace epl_backend.Models.DTOs;

public class ClubDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Founded { get; set; }
    public string? City { get; set; }
    public string? Stadium { get; set; }
    public string? HeadCoach { get; set; }
    public string? ClubOfficialWebsite { get; set; }
    public string? Crest { get; set; }
    public string? Theme { get; set; }
    
    public IFormFile? CrestFile { get; set; }
}
