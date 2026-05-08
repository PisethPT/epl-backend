using System.ComponentModel.DataAnnotations;

namespace PremierLeague_Backend.Models.DTOs;

public class NewsDto
{
    public int? NewsId { get; set; }
    [Required(ErrorMessage = "News Tag is required")]

    public int NewsCategoryId { get; set; }
    public int NewsTagId { get; set; }
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    [RegularExpression(@"^\S+(\s+\S+)*$", ErrorMessage = "Title cannot be empty or only whitespace.")]
    public string Title { get; set; }
    [MaxLength(500, ErrorMessage = "Subtitle cannot exceed 500 characters")]
    [RegularExpression(@"^\S+(\s+\S+)*$", ErrorMessage = "Subtitle cannot be empty or only whitespace.")]
    public string? Subtitle { get; set; }
    [Required(ErrorMessage = "Content is required")]
    [MaxLength(4000, ErrorMessage = "Content cannot exceed 4000 characters")]
    [RegularExpression(@"^\S+(\s+\S+)*$", ErrorMessage = "Content cannot be empty or only whitespace.")]
    public string Content { get; set; }
    public string? ReferenceUrl { get; set; }
    public string? VideoReferenceUrl { get; set; }
    public string? AuthorId { get; set; }
    [Required(ErrorMessage = "Published Date is required")]
    [DataType(DataType.Date)]
    public DateTime PublishedDate { get; set; } = DateTime.Now;
    public string? ImageUrl { get; set; }
    [Required(ErrorMessage = "Expiry Date is required")]
    [DataType(DataType.Date)]
    public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(7);
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public bool IsVideo { get; set; } = false;
    public bool IsQuizzes { get; set; } = false;
    public bool IsRelatedContent { get; set; } = false;
    public bool IsPremierLeagueGame { get; set; } = false;
    public int? MatchId { get; set; }
    public List<int> ClubIds { get; set; } = new();
    public List<int> PlayerIds { get; set; } = new();

    // [Required(ErrorMessage = "News Image is required")]
    // [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }
}
