using epl_backend.Models.SelectListItems;

namespace epl_backend.Data.Repositories.Interfaces;

public interface ISelectListItems
{
    Task<List<SelectListItemClub>> SelectListItemClubAsync(CancellationToken ct = default);
    Task<List<SelectListItemSeason>> SelectListItemSeasonAsync(CancellationToken ct = default);
    Task<List<SelectListItemPlayerLineupByClubId>> SelectListItemPlayerLineupByClubIdAsync(int matchId , int clubId, CancellationToken ct = default);
    Task<List<SelectListItemReferee>> SelectListItemRefereeAsync(CancellationToken ct = default);
    Task<List<SelectListItemRefereeRole>> SelectListItemRefereeRoleAsync(CancellationToken ct = default);
    Task<List<SelectListItemRefereeBadgeLevel>> SelectListItemRefereeBadgeLevelAsync(CancellationToken ct = default);
    Task<List<SelectListItemMatchForLineup>> SelectListItemMatchForLineupAsync(CancellationToken ct = default);
    Task<List<SelectListItemFormation>> SelectListItemFormationAsync(CancellationToken ct = default);
}
