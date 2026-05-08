using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.ViewModels;
using PremierLeague_Backend.Helper.SqlCommands;
using PremierLeague_Backend.Models.SelectListItems;
using PremierLeague_Backend.Helper;
using System.Security.Claims;

namespace PremierLeague_Backend.Controllers;

[Authorize]
[Route("en/match-info")]
public class MatchInfoController : Controller
{
    private readonly ILogger<MatchInfoController> _logger;
    private readonly IMatchInfoRepository repository;
    private readonly ISelectListItems selectListItems;
    private MatchInfoViewModel viewModel;

    public MatchInfoController(ILogger<MatchInfoController> logger, IMatchInfoRepository repository, ISelectListItems selectListItems)
    {
        _logger = logger;
        this.repository = repository;
        this.selectListItems = selectListItems;
        this.viewModel = new MatchInfoViewModel();
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync(int page = 1)
    {
        try
        {
            int pageSize = 20;
            viewModel.matchInfos = await repository.GetAllAsync(page);

            int totalCount = await repository.GetCountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching match info list: {Message}", ex.Message);
            ModelState.AddModelError(string.Empty, "Unable to load match info list. Please try again later or contact admin.");
        }
        return View(viewModel);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAsync([FromForm] MatchInfoDto matchInfoDto)
    {
        try
        {
            matchInfoDto.AuthorId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "12c64674-5ea2-484d-90b1-7419243b1758";

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value!.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                throw new Exception($"Form validation failed: {string.Join(" | ", errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"))}");
            }

            var success = await repository.AddAsync(matchInfoDto);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to create match info. Please try again or contact admin.");
                _logger.LogWarning("CreateMatchInfoAsync returned {success} for {MatchId}", success, matchInfoDto.MatchId);
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Match Info create successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating match info: {Message}", ex.Message);
            ModelState.AddModelError(string.Empty, "Unable to create match info. Please try again later or contact admin.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("update/{matchInfo:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAsync([FromRoute] int matchInfo, [FromForm] MatchInfoDto matchInfoDto)
    {
        try
        {
            matchInfoDto.AuthorId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "12c64674-5ea2-484d-90b1-7419243b1758";
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value!.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                throw new Exception($"Form validation failed: {string.Join(" | ", errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"))}");
            }

            var success = await repository.UpdateAsync(matchInfoDto);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to update match info. Please try again or contact admin.");
                _logger.LogWarning("UpdateMatchInfoAsync returned {success} for {MatchId}", success, matchInfoDto.MatchId);
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Match Info updated successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating match info: {Message}", ex.Message);
            ModelState.AddModelError(string.Empty, "Unable to update match info. Please try again later or contact admin.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAsync(int matchInfoId)
    {
        try
        {
            if (matchInfoId == 0 || await repository.FindByIdAsync(matchInfoId) is null) throw new Exception("Match info does not exist.");

            var success = await repository.DeleteAsync(matchInfoId);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete match info. Please try again or contact admin.");
                _logger.LogWarning("DeleteAsync returned {success} for match info ID {MatchInfoId}", success, matchInfoId);
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Match info deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting season ID {MatchInfoId}", matchInfoId);
            ModelState.AddModelError(string.Empty, ex.Message);
            viewModel.matchInfos = await repository.GetAllAsync(page: 1);
            return View(nameof(Index), viewModel);
        }
    }

    [HttpGet("get-match-info/{matchInfoId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetMatchInfoAsync([FromRoute] int matchInfoId)
    {
        try
        {
            var matchInfoDto = await repository.FindByIdAsync(matchInfoId);

            return Json(new
            {
                StatusCode = 200,
                Message = "Commit Transaction Success.",
                Data = new
                {
                    Item = matchInfoDto
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting match info by id");
            ModelState.AddModelError(string.Empty, ex.Message);
            return Json(new
            {
                StatusCode = 400,
                Message = ex.Message,
            });
        }
    }


    [HttpGet("get-match/{isEdit:bool}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetMatchAsync([FromRoute] bool isEdit)
    {
        try
        {
            var items = await selectListItems.SelectListItemAsync<SelectListItemMatch>("PL_SelectListItemMatch_MatchInfo",
            new Dictionary<string, string>() { { "@IsEdit", isEdit.ToString().ToLower() } },
             rdr => new SelectListItemMatch
            (
                rdr.SafeGetInt("MatchId"),
                rdr.SafeGetInt("HomeClubId"),
                rdr.SafeGetInt("AwayClubId"),
                rdr.SafeGetString("HomeClubCrest"),
                rdr.SafeGetString("AwayClubCrest"),
                rdr.SafeGetString("HomeClubTheme"),
                rdr.SafeGetString("AwayClubTheme"),
                rdr.SafeGetString("MatchContent"),
                rdr.SafeGetString("MatchSubContent")
            ));

            return Json(new
            {
                StatusCode = 200,
                Message = "Commit Transaction Success.",
                Data = new
                {
                    Item = items
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting news by id");
            ModelState.AddModelError(string.Empty, ex.Message);
            return Json(new
            {
                StatusCode = 400,
                Message = ex.Message,
            });
        }
    }

    [HttpPost("get-match-player")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPlayersAsync(bool isHomeClub, int matchId)
    {
        try
        {
            var items = await selectListItems.SelectListItemAsync<SelectListItemHasImage>("PL_SelectListItemPlayerForManOfTheMatch",
            new Dictionary<string, string>() { { "@IsHomeClub", isHomeClub.ToString().ToLower() }, { "@MatchId", matchId.ToString() } },
             rdr => new SelectListItemHasImage
             {
                 Value = rdr.SafeGetInt("PlayerId"),
                 Label = rdr.SafeGetString("PlayerName"),
                 Img = rdr.SafeGetString("Photo"),
             });

            return Json(new
            {
                StatusCode = 200,
                Message = "Commit Transaction Success.",
                Data = new
                {
                    Item = items
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting players by id");
            ModelState.AddModelError(string.Empty, ex.Message);
            return Json(new
            {
                StatusCode = 400,
                Message = ex.Message,
            });
        }
    }
}