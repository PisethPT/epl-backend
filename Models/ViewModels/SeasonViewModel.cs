using epl_backend.Models.DTOs;

namespace epl_backend.Models.ViewModels;

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
