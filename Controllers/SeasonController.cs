using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace PremierLeague_Backend.Controllers
{
    [Route("seasons")]
    public class SeasonController : Controller
    {
        private readonly ISeasonRepository repository;
        private SeasonViewModel viewModel { get; set; }
        private readonly ILogger<SeasonController> _logger;
        public SeasonController(ISeasonRepository repository, ILogger<SeasonController> logger)
        {
            this.repository = repository;
            this.viewModel = new();
            _logger = logger;
        }

        // GET: SeasonController
        public async Task<ActionResult> Index(int? page)
        {
            viewModel.seasonDetailDtos = await repository.GetAllSeasonAsync(page);
            return View(viewModel);
        }

        // POST: SeasonController/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] SeasonDto seasonDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                    throw new Exception($"Form validation failed: {errors}");
                }

                if (!await repository.SeasonExistingAsync(seasonDto)) throw new Exception($"Season name '{seasonDto.SeasonName}' already exists. Please choose a different name.");

                var success = await repository.AddSeasonAsync(seasonDto);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to create season. Please try again or contact admin.");
                    _logger.LogWarning("AddSeasonAsync returned {success} for season {SeasonName}", success, seasonDto.SeasonName);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Season created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding season {SeasonName}", seasonDto.SeasonName);
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.seasonDetailDtos = await repository.GetAllSeasonAsync(page: 1);
                return View(nameof(Index), viewModel);
            }
        }

        // POST: SeasonController/Update
        [HttpPost("update/{seasonId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update([FromForm] SeasonDto seasonDto, [FromRoute] int seasonId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                    throw new Exception($"Form validation failed: {errors}");
                }

                if (!await repository.SeasonExistingAsync(seasonDto, seasonId)) throw new Exception($"Season name '{seasonDto.SeasonName}' already exists. Please choose a different name.");

                var success = await repository.UpdateSeasonAsync(seasonDto);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to update season. Please try again or contact admin.");
                    _logger.LogWarning("UpdateSeasonAsync returned {success} for season ID {SeasonId}", success, seasonDto.SeasonId);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Season updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating season ID {SeasonId}", seasonDto.SeasonId);
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.seasonDetailDtos = await repository.GetAllSeasonAsync(page: 1);
                return View(nameof(Index), viewModel);
            }
        }

        // POST: SeasonController/Delete
        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int seasonId)
        {
            try
            {
                if (await repository.FindSeasonByIdAsync(seasonId) is null) throw new Exception("Season does not exist.");

                var success = await repository.DeleteSeasonAsync(seasonId);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to delete season. Please try again or contact admin.");
                    _logger.LogWarning("DeleteSeasonAsync returned {success} for season ID {SeasonId}", success, seasonId);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Season deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting season ID {SeasonId}", seasonId);
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.seasonDetailDtos = await repository.GetAllSeasonAsync(page: 1);
                return View(nameof(Index), viewModel);
            }
        }

        // POST: SeasonController/FindSeasonById
        [HttpPost("get-season/{seasonId}")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> FindSeasonById([FromRoute] int seasonId)
        {
            try
            {
                var seasonDto = await repository.FindSeasonByIdAsync(seasonId);
                if (seasonDto is null)
                {
                    ModelState.AddModelError(string.Empty, "Season not found.");
                    return Json(new { error = "Season not found." });
                }
                return Json(seasonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding season ID {SeasonId}", seasonId);
                return Json(new { error = ex.Message });
            }
        }
    }
}