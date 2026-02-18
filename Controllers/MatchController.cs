using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Helper.SqlCommands;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PremierLeague_Backend.Helper.Validation;

namespace PremierLeague_Backend.Controllers
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
        public async Task<ActionResult> Index(List<int> club, int season, int week = 0, int? page = 1)
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

                season = season == 0 ? matchSeasonDto.SeasonId : season;

                if (week == 0)
                {
                    //(week, seasonId) = await repository.GetCurrentMatchWeekAndSeasonIdAsync();
                    week = matchSeasonDto.MatchWeek;
                }

                viewModel.SelectedClubIds = club;
                var clubIdJson = System.Text.Json.JsonSerializer.Serialize(club);

                viewModel.matchDetailDtos = await repository.GetAllMatchAsync(season, week, page, clubIdJson: clubIdJson);
                viewModel.MatchWeekDto = await repository.GetMatchWeekBySeasonIdAsync(season, week);
                viewModel.SelectListItemSeasons = await selectList.SelectListItemSeasonAsync();
                viewModel.SelectListItemReferees = await selectList.SelectListItemRefereeAsync();

                viewModel.MatchWeekSelectListItem = await selectList.SelectListItemsAsync(SelectListItemCommands.SelectListItemMatchWeekCommandText);
                viewModel.SelectListItemClubs = await selectList.SelectListItemClubAsync();
                viewModel.SeasonsSelectListItem = await selectList.SelectListItemsAsync(SelectListItemCommands.SelectListItemSeasonCommandText);

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

        // POST: MatchController/Create
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

                if (!await repository.ValidateMatchAsync(matchDto, ModelState))
                {
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

        // POST: MatchController/Update/5
        [HttpPost("update/{matchId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync([FromForm] MatchDto matchDto, [FromRoute] int matchId)
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

                if (!await repository.ValidateMatchAsync(matchDto, ModelState))
                {
                    return RedirectToAction(nameof(Index));
                }

                bool isUpdated = await repository.UpdateMatchAsync(matchDto);
                if (!isUpdated)
                {
                    ModelState.AddModelError(string.Empty, "Unable to update match. Please try again or contact admin.");
                    _logger.LogWarning("UpdateMatchAsync returned {success} for match {MatchId}", isUpdated, matchDto.MatchId);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = $"Match on {matchDto.MatchDate} updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating match on {Date}", matchDto.MatchDate);
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MatchController/Delete/5
        [HttpPost("delete/{matchId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int matchId)
        {
            try
            {
                var matchDto = await repository.FindMatchByIdAsync(matchId);
                if (matchDto is null) throw new Exception("Match does not exist.");

                bool isDeleted = await repository.DeleteMatchAsync(matchId);
                if (!isDeleted)
                {
                    ModelState.AddModelError(string.Empty, "Unable to delete match. Please try again or contact admin.");
                    _logger.LogWarning("DeleteMatchAsync returned {success} for match {MatchId}", isDeleted, matchId);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = $"Match on {matchDto.MatchDate} deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting match ID {MatchId}", matchId);
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MatchController/GetMatch/5
        [HttpPost("get-match/{matchId}")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetMatchByIdAsync([FromRoute] int matchId)
        {
            try
            {
                var matchDto = await repository.FindMatchByIdAsync(matchId);
                if (matchDto is null) throw new Exception("Match does not exist.");
                matchDto.MatchReferees = await repository.FindMatchRefereeByMatchIdAsync(matchId);
                if (matchDto.MatchReferees is null) matchDto.MatchReferees = new List<MatchRefereeDto>();

                return Json(matchDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error find match on {MatchId}", matchId);
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new
                {
                    StatusCode = 400,
                    Message = ex.Message,
                });
            }
        }

        [HttpPost("get-match-details/{matchId}")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetMatchDetailByMatchIdAsync([FromRoute] int matchId)
        {
            try
            {
                var matchDetail = await repository.GetMatchInfoMatchDetailsByMatchIdAsync(matchId);
                var matchOfficials = await repository.GetMatchInfoMatchOfficialsByMatchIdAsync(matchId);
                return Json(new
                {
                    StatusCode = 200,
                    Message = "Commit Transaction Success.",
                    Data = new
                    {
                        matchDetail,
                        matchOfficials
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error find match on {MatchId}", matchId);
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new
                {
                    StatusCode = 400,
                    Message = ex.Message,
                });
            }
        }
    }
}
