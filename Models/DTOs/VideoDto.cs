using System.ComponentModel.DataAnnotations;

namespace PremierLeague_Backend.Models.DTOs;

public class VideoDto
{
    public int? VideoId { get; set; }
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; }
    [MaxLength(4000, ErrorMessage = "Description cannot exceed 4000 characters")]
    public string? Description { get; set; }
    [MaxLength(100, ErrorMessage = "Channel cannot exceed 100 characters")]
    public string? Channel { get; set; }
    [MaxLength(500, ErrorMessage = "Thumbnail URL cannot exceed 500 characters")]
    public string? ThumbnailUrl { get; set; }
    [MaxLength(500, ErrorMessage = "Video URL cannot exceed 500 characters")]
    public string? VideoUrl { get; set; }
    public string? ReferenceUrl { get; set; }
    [Required(ErrorMessage = "Video Category ID is required")]
    public int VideoCategoryId { get; set; }
    public int? VideoTagId { get; set; }
    public string? Publisher { get; set; }
    [Required(ErrorMessage = "Duration is required")]
    public decimal DurationSeconds { get; set; }
    [Required(ErrorMessage = "Published Date is required")]
    public DateTime PublishedDate { get; set; }
    [Required(ErrorMessage = "Expiry Date is required")]
    public DateTime ExpiryDate { get; set; }
    public bool IsStory { get; set; } = false;
    public bool IsReference { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsTheArchive { get; set; } = false;
    public int? MatchId { get; set; }
    public int? SeasonId { get; set; }

    public List<int> ClubIds { get; set; } = new();
    public List<int> PlayerIds { get; set; } = new();
}
