using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Models.ViewModels;

public class SeasonViewModel
{
    public SeasonDto seasonDto { get; set; }
    public List<SeasonDetailDto> seasonDetailDtos { get; set; }
    public SeasonViewModel()
    {
        this.seasonDto = new();
        this.seasonDetailDtos = new();
    }
}
