using Microsoft.AspNetCore.Mvc.Rendering;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Models.SelectListItems;

namespace PremierLeague_Backend.Models.ViewModels;

public class NewsViewModel
{
    public NewsDto NewsDto { get; set; }
    public IEnumerable<NewsDetailDto> NewsDetailDtos { get; set; }
    public IEnumerable<SelectListItemClub> SelectListItemClubs { get; set; }
    public IEnumerable<PlayerStatGetPlayersDto> SelectListItemPlayers { get; set; }
    public IEnumerable<SelectListItemMatch> SelectListItemMatches { get; set; }
    public List<SelectListItemHasSubtitle> SelectListItemNewsTag { get; set; }
    public List<SelectListItem> SelectListItemNewsCategories { get; set; }
    public NewsViewModel()
    {
        this.NewsDto = new();
        this.NewsDetailDtos = new List<NewsDetailDto>();
        this.SelectListItemNewsTag = new List<SelectListItemHasSubtitle>();
        this.SelectListItemNewsCategories = new List<SelectListItem>();
        this.SelectListItemClubs = new List<SelectListItemClub>();
        this.SelectListItemPlayers = new List<PlayerStatGetPlayersDto>();
        this.SelectListItemMatches = new List<SelectListItemMatch>();
    }
}
