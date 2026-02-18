using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace PremierLeague_Backend.Controllers
{
    [Route("lineups")]
    public class LineupController : Controller
    {
        private readonly ILogger<LineupController> _logger;
        private readonly ILineupRepository repository;
        private readonly ISelectListItems selectList;
        private readonly LineupViewModel viewModel;

        public LineupController(ILogger<LineupController> logger, ILineupRepository repository, ISelectListItems selectList)
        {
            this._logger = logger;
            this.repository = repository;
            this.selectList = selectList;
            this.viewModel = new();
        }

        // GET: LineupController
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            viewModel.SelectListItemMatchForLineups = await selectList.SelectListItemMatchForLineupAsync();
            return View(viewModel);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync()
        {
            return View(nameof(Index));
        }

        [HttpGet("get-formations")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetFormations()
        {
            try
            {
                viewModel.SelectListItemFormations = await selectList.SelectListItemFormationAsync();

                return Json(
                    new
                    {
                        StatusCode = 200,
                        Data = new
                        {
                            ListItem = viewModel.SelectListItemFormations,
                        },
                        Message = "Commit Transaction Success."
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading formations");
                return Json(
                    new
                    {
                        StatusCode = 400,
                        Message = ex.Message,
                    }
                );
            }
        }

        [HttpPost("get-players-by-match/{matchId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPlayersByMatchAsync([FromRoute] int matchId)
        {
            try
            {
                var (homeClubId, awayClubId) = await repository.GetHomeClubAndAwayClubByMatchIdAsync(matchId);
                if (homeClubId == 0 || awayClubId == 0)
                {
                    throw new Exception("Match not found.");
                }

                viewModel.SelectListItemHomeClubPlayer = await selectList.SelectListItemPlayerLineupByClubIdAsync(matchId, homeClubId);
                viewModel.SelectListItemAwayClubPlayer = await selectList.SelectListItemPlayerLineupByClubIdAsync(matchId, awayClubId);

                return Json(
                    new
                    {
                        StatusCode = 200,
                        Data = new
                        {
                            HomePlayers = viewModel.SelectListItemHomeClubPlayer,
                            AwayPlayers = viewModel.SelectListItemAwayClubPlayer,
                        },
                        Message = "Commit Transaction Success."
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading players by match Id");
                return Json(
                    new
                    {
                        StatusCode = 400,
                        Message = ex.Message,
                    }
                );
            }
        }

    }
}
