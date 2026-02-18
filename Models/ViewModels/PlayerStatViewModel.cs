using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Models.ViewModels;

public class PlayerStatViewModel
{
    public IEnumerable<PlayerStatMatchListDto> PlayerStatMatchListDtos { get; set; }
    public PlayerStatDto PlayerStatDto { get; set; }
    public PlayerStatViewModel()
    {
        this.PlayerStatDto = new PlayerStatDto();
    }
}
