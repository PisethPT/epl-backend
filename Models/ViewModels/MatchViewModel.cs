using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Models.SelectListItems;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PremierLeague_Backend.Models.ViewModels;

public class MatchViewModel
{
    public List<MatchDetailDto> matchDetailDtos { get; set; }
    public MatchDto matchDto { get; set; }
    public MatchSettingsDto matchSettingsDto { get; set; }
    public MatchSeasonDto matchSeasonDto { get; set; }
    public List<SelectListItemClub> SelectListItemClubs { get; set; }
    public List<SelectListItemSeason> SelectListItemSeasons { get; set; }
    public List<SelectListItemPlayerLineupByClubId> SelectListItemPlayer { get; set; }
    public List<SelectListItemReferee> SelectListItemReferees { get; set; }
    public List<SelectListItem> MatchWeekSelectListItem { get; set; }
    public List<SelectListItem> SeasonsSelectListItem { get; set; }
    public MatchWeekDto MatchWeekDto { get; set; }
    public List<int> SelectedClubIds { get; set; } = new();

    public MatchInfoMatchDetailsDto? MatchDetailsDto { get; set; }
    public List<MatchInfoMatchOfficialsDto> MatchOfficialsDtos { get; set; }

    public MatchViewModel()
    {
        this.matchDetailDtos = new();
        this.matchDto = new();
        this.matchSettingsDto = new();
        this.matchSeasonDto = new();
        this.SelectListItemClubs = new();
        this.SelectListItemSeasons = new();
        this.SelectListItemPlayer = new();
        this.SelectListItemReferees = new();
        this.MatchWeekSelectListItem = new();
        this.SeasonsSelectListItem = new();
        this.MatchOfficialsDtos = new();

        this.MatchWeekDto = new();
    }
}
