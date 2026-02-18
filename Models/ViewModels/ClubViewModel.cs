using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Models.ViewModels;

public class ClubViewModel
{
    public ClubDto clubDto { get; set; }
    public List<ClubDto> clubDtos { get; set; }

    public ClubViewModel()
    {
        this.clubDto = new();
        this.clubDtos = new();
    }
}
