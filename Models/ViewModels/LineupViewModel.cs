using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Models.SelectListItems;

namespace PremierLeague_Backend.Models.ViewModels;

public class LineupViewModel
{
    public LineupDto LineupDto { get; set; }
    public List<SelectListItemMatchForLineup> SelectListItemMatchForLineups { get; set; }
    public List<SelectListItemPlayerLineupByClubId> SelectListItemHomeClubPlayer { get; set; }
    public List<SelectListItemPlayerLineupByClubId> SelectListItemAwayClubPlayer { get; set; }
    public List<SelectListItemFormation> SelectListItemFormations { get; set; }
    public LineupViewModel()
    {
        this.LineupDto = new();
        this.SelectListItemMatchForLineups = new();
        this.SelectListItemHomeClubPlayer = new();
        this.SelectListItemAwayClubPlayer = new();
        this.SelectListItemFormations = new();
    }
}
