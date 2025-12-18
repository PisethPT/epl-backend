using epl_backend.Models.SelectListItems;

namespace epl_backend.Data.Repositories.Interfaces;

public interface ISelectListItems
{
    Task<List<SelectListItemClub>> SelectListItemClubAsync(CancellationToken ct = default);
}
