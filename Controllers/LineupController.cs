using System.Threading.Tasks;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace epl_backend.Controllers
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
            viewModel.SelectListItemHomeClubPlayer = await selectList.SelectListItemPlayerLineupByClubIdAsync(1, 1);
            viewModel.SelectListItemAwayClubPlayer = await selectList.SelectListItemPlayerLineupByClubIdAsync(1, 3);

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
