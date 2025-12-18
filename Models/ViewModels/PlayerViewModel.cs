using epl_backend.Models.DTOs;
using epl_backend.Models.SelectListItems;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace epl_backend.Models.ViewModels;

public class PlayerViewModel
{
    public PlayerDto PlayerDto { get; set; }
    public List<PlayerDetailDto> PlayerDetailDtos { get; set; }
    public List<SelectListItem> JoinedClubSelectListItem = new();
    public List<SelectListItemClub> SelectListItemClubs { get; set; }
    public List<int> SelectedPositions { get; set; } = new();
    public List<int> SelectedClubIds { get; set; } = new();

    public PlayerViewModel()
    {
        PlayerDto = new();
        PlayerDetailDtos = new();
        SelectListItemClubs = new();

        for (int year = DateTime.Now.Year; year > 2012; year--)
        {
            int next = year + 1;
            string season = $"{year}/{next.ToString().Substring(2)}";

            JoinedClubSelectListItem.Add(new SelectListItem
            {
                Value = season,
                Text = season
            });
        }
    }
}
