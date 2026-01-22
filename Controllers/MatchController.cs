using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Models.DTOs;
using epl_backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace epl_backend.Controllers
{
    [Route("matches")]
    public class MatchController : Controller
    {
        private readonly ILogger<MatchController> _logger;
        private readonly IMatchRepository repository;
        private readonly ISelectListItems selectList;
        private readonly MatchViewModel viewModel;
        public MatchController(ILogger<MatchController> logger, IMatchRepository repository, ISelectListItems selectList)
        {
            _logger = logger;
            this.repository = repository;
            this.selectList = selectList;
            this.viewModel = new MatchViewModel();
        }

        // GET: MatchController
        [HttpGet]
        public async Task<ActionResult> Index(int seasonId, int week = 1, int? page = 1)
        {
            try
            {
                int pageSize = 20;
                var matchSeasonDto = await repository.GetMatchWeekAsync();
                if (matchSeasonDto is null)
                {
                    ModelState.AddModelError(nameof(matchSeasonDto), "No active season found. Please set an active season before creating matches.");
                    return RedirectToAction(nameof(Index));
                }

                seasonId = seasonId == 0 ? matchSeasonDto.SeasonId : seasonId;

                viewModel.matchDetailDtos = await repository.GetAllMatchAsync(seasonId, week, page);
                viewModel.MatchWeekDto = await repository.GetMatchWeekBySeasonIdAsync(seasonId, week);
                viewModel.SelectListItemClubs = await selectList.SelectListItemClubAsync();
                viewModel.SelectListItemSeasons = await selectList.SelectListItemSeasonAsync();
                viewModel.SelectListItemPlayer = await selectList.SelectListItemPlayerLineupByClubIdAsync(1, 1);

                viewModel.SelectListItemReferees = await selectList.SelectListItemRefereeAsync();

                int totalCount = viewModel.matchDetailDtos.Count(); //await repository.CountMatchesAsync(positionJson, clubIdJson);
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                viewModel.matchDto.MatchWeek = matchSeasonDto.MatchWeek;
                viewModel.matchDto.SeasonId = matchSeasonDto.SeasonId;

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading matches with filters");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([FromForm] MatchDto matchDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value!.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                var matchSeasonDto = await repository.GetMatchWeekAsync();
                if (matchSeasonDto is null)
                {
                    ModelState.AddModelError(nameof(matchSeasonDto), "No active season found. Please set an active season before creating matches.");
                    return RedirectToAction(nameof(Index));
                }

                matchDto.SeasonId = matchSeasonDto.SeasonId;
                matchDto.MatchWeek = matchDto.MatchWeek;

                var isMatchDateCannotBeEarlierThanToday = await repository.FindMatchDateCannotBeEarlierThanToday(matchDto.MatchDate);
                if (!isMatchDateCannotBeEarlierThanToday)
                {
                    ModelState.AddModelError(nameof(matchDto.MatchDate), "Match date cannot be earlier than today.");
                    return RedirectToAction(nameof(Index));
                }

                var isMatchDateIsOutsideTheActionSeason = await repository.FindMatchDateIsOutsideTheActiveSeason(matchDto.MatchDate, matchSeasonDto.StartSeasonDate, matchSeasonDto.EndSeasonDate);
                if (!isMatchDateIsOutsideTheActionSeason)
                {
                    ModelState.AddModelError(nameof(matchDto.MatchDate), "Match date is outside the active season dates.");
                    return RedirectToAction(nameof(Index));
                }

                var isMatchAlreadyExistsOnTheSelectedDate = await repository.FindMatchAlreadyExistsOnTheSelectedDate(matchDto.MatchDate, matchDto.HomeClubId, matchDto.AwayClubId);
                if (!isMatchAlreadyExistsOnTheSelectedDate)
                {
                    ModelState.AddModelError(nameof(matchDto.MatchDate), "Match already exists on the selected date.");
                    return RedirectToAction(nameof(Index));
                }

                var isClubCannotBeTheSameAsync = await repository.FindClubCannotBeTheSameAsync(matchDto.HomeClubId, matchDto.AwayClubId);
                if (!isClubCannotBeTheSameAsync)
                {
                    ModelState.AddModelError(nameof(matchDto.MatchDate), "Home and away clubs cannot be the same.");
                    return RedirectToAction(nameof(Index));
                }

                bool isCreated = await repository.AddMatchAsync(matchDto);
                if (!isCreated)
                {
                    ModelState.AddModelError(string.Empty, "Unable to create match. Please try again or contact admin.");
                    _logger.LogWarning("AddMatchAsync returned {success} for match {MatchId}", isCreated, matchDto.MatchId);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = $"Match on {matchDto.MatchDate} create successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating match on {Date}", matchDto.MatchDate);
                ModelState.AddModelError(string.Empty, ex.Message);
                var matchSeasonDto = await repository.GetMatchWeekAsync();
                viewModel.matchDetailDtos = await repository.GetAllMatchAsync(seasonId: matchSeasonDto.SeasonId, week: matchSeasonDto.MatchWeek, page: 1);
                return View(nameof(Index), viewModel);
            }
        }

    }
}
