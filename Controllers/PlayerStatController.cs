using Microsoft.AspNetCore.Mvc;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.ViewModels;

namespace PremierLeague_Backend.Controllers
{
    [Route("player-stats")]
    public class PlayerStatController : Controller
    {
        private readonly ILogger<PlayerStatController> _logger;
        private readonly IPlayerStatRepository repository;
        private readonly PlayerStatViewModel viewModel;

        public PlayerStatController(ILogger<PlayerStatController> logger ,IPlayerStatRepository repository)
        {
            this._logger = logger;
            this.repository = repository;
            this.viewModel = new PlayerStatViewModel();
        }

        // GET: PlayerStatController
        [HttpGet]
        public async Task<ActionResult> Index(List<int>? club, int season, int week = 0, int? page = 1)
        {
            try
            {
                viewModel.PlayerStatMatchListDtos = await repository.GetPlayerStatMatchListAsync(season, week, page);
                return View(viewModel);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error loading player stats with filters");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }
        }

    }
}
