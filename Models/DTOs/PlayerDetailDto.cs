using epl_backend.Models.Enums;

namespace epl_backend.Models.DTOs;

public class PlayerDetailDto
{
    public int PlayerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string PlaceOfBirth { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string NationalityIcon { get; set; } = string.Empty;
    public PreferredFoot PreferredFoot { get; set; }
    public string Height { get; set; } = string.Empty;
    public Position Position { get; set; }
    public int PlayerNumber { get; set; }
    public string JoinedClub { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public int ClubId { get; set; }
    public string ClubName { get; set; } = string.Empty;
    public string ClubCrest { get; set; } = string.Empty;
    public string ClubTheme { get; set; } = string.Empty;
}
