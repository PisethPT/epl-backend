using epl_backend.Models.DTOs;
using epl_backend.Models.SelectListItems;

namespace epl_backend.Models.ViewModels;

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
    public List<SelectListItemRefereeRole> SelectListItemRefereeRoles { get; set; }
    public List<SelectListItemRefereeBadgeLevel> SelectListItemRefereeBadgeLevels { get; set; }
    public MatchWeekDto MatchWeekDto { get; set; }
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
        this.SelectListItemRefereeRoles = new();
        this.SelectListItemRefereeBadgeLevels = new();

        this.MatchWeekDto = new();
    }
}
