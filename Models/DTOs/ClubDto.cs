using System;
using System.ComponentModel.DataAnnotations;

namespace epl_backend.Models.DTOs;

public class ClubDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Club name is required.")]
    public string? Name { get; set; }

    public string? Founded { get; set; }

    public string? City { get; set; }

    public string? Stadium { get; set; }

    public string? HeadCoach { get; set; }

    [Url(ErrorMessage = "Please enter a valid website URL.")]
    public string? ClubOfficialWebsite { get; set; }

    public string? Crest { get; set; }

    [Required(ErrorMessage = "Theme color is required.")]
    public string? Theme { get; set; }

    [Required(ErrorMessage = "Please upload a crest image.")]
    public IFormFile? CrestFile { get; set; }
}

