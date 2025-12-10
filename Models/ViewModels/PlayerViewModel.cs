using epl_backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace epl_backend.Models.ViewModels;

public class PlayerViewModel
{
    public PlayerDto PlayerDto { get; set; }
    public List<SelectListItem> JoinedClubSelectListItem = new();

    public PlayerViewModel()
    {
        PlayerDto = new();
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
