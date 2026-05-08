using Microsoft.AspNetCore.Mvc.Rendering;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Models.SelectListItems;

namespace PremierLeague_Backend.Models.ViewModels;

public class VideoViewModel
{
    public IEnumerable<VideoDetailDto> VideoDetailDtos { get; set; }
    public IEnumerable<SelectListItemClub> SelectListItemClubs { get; set; }
    public IEnumerable<PlayerStatGetPlayersDto> SelectListItemPlayers { get; set; }
    public IEnumerable<SelectListItemMatch> SelectListItemMatches { get; set; }
    public VideoDto VideoDto { get; set; }
    public IEnumerable<SelectListItem> selectListItemVideoTag { get; set; }
    public IEnumerable<SelectListItem> SelectListItemSeason { get; set; }
    public VideoViewModel()
    {
        this.VideoDetailDtos = new List<VideoDetailDto>();
        this.VideoDto = new VideoDto();
        this.selectListItemVideoTag = new List<SelectListItem>();
        this.SelectListItemSeason = new List<SelectListItem>();
        this.SelectListItemClubs = new List<SelectListItemClub>();
        this.SelectListItemPlayers = new List<PlayerStatGetPlayersDto>();
        this.SelectListItemMatches = new List<SelectListItemMatch>();
    }
}
