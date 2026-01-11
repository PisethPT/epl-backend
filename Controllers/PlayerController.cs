using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Helper;
using epl_backend.Models.DTOs;
using epl_backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace epl_backend.Controllers
{
    [Route("players")]
    public class PlayerController : Controller
    {
        private readonly PlayerViewModel viewModel;
        private readonly ILogger<PlayerController> _logger;
        private readonly ISelectListItems selectList;
        private readonly IPlayerRepository repository;
        private readonly IWebHostEnvironment env;

        public PlayerController(ILogger<PlayerController> logger, ISelectListItems selectList, IPlayerRepository repository, IWebHostEnvironment env)
        {
            viewModel = new PlayerViewModel();
            _logger = logger;
            this.selectList = selectList;
            this.repository = repository;
            this.env = env;
        }

        // GET: PlayerController
        [HttpGet]
        public async Task<ActionResult> Index(List<int> positions, List<int> clubIds, int page = 1)
        {
            try
            {
                clubIds ??= new();
                positions ??= new();

                int pageSize = 20;
                viewModel.SelectedPositions = positions;
                viewModel.SelectedClubIds = clubIds;

                var positionJson = System.Text.Json.JsonSerializer.Serialize(positions);
                var clubIdJson = System.Text.Json.JsonSerializer.Serialize(clubIds);

                viewModel.SelectListItemClubs = await selectList.SelectListItemClubAsync();
                viewModel.PlayerDetailDtos = await repository.GetAllPlayerAsync(positionJson, clubIdJson, page);

                int totalCount = await repository.CountPlayersAsync(positionJson, clubIdJson);
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                var jsonPath = Path.Combine(env.WebRootPath, "json", "nationality.json");
                var natFile = NationalityLoader.LoadFromPath(jsonPath);

                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (natFile?.nationalities != null)
                {
                    foreach (var n in natFile.nationalities)
                    {
                        if (!string.IsNullOrWhiteSpace(n.nationality) && !string.IsNullOrWhiteSpace(n.icon))
                            dict[n.nationality.Trim()] = n.icon.Trim();

                        if (!string.IsNullOrWhiteSpace(n.name) && !string.IsNullOrWhiteSpace(n.icon))
                            dict[n.name.Trim()] = n.icon.Trim();
                    }
                }

                foreach (var p in viewModel.PlayerDetailDtos)
                {
                    p.NationalityIcon = "https://flagcdn.com/w40/un.png";

                    if (!string.IsNullOrWhiteSpace(p.Nationality) &&
                        dict.TryGetValue(p.Nationality.Trim(), out var icon))
                    {
                        p.NationalityIcon = icon;
                    }
                    else
                    {
                        var found = natFile?.nationalities?.FirstOrDefault(x =>
                            string.Equals(x.nationality, p.Nationality, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(x.name, p.Nationality, StringComparison.OrdinalIgnoreCase));

                        if (found != null)
                            p.NationalityIcon = found.icon;
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading players with filters");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }
        }

        // POST: PlayerController/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] PlayerDto playerDto)
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

                var validation = FileValidator.Validate(playerDto.PhotoFile);
                if (!validation.IsValid)
                {
                    ModelState.AddModelError(nameof(playerDto.PhotoFile), validation.ErrorMessage ?? "Invalid file");
                    return RedirectToAction(nameof(Index));
                }

                if (!await repository.PlayerExistingByClubAsync(playerDto)) throw new Exception("This player is already registered in this club.");

                var success = await repository.AddPlayerAsync(playerDto);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to create player. Please try again or contact admin.");
                    _logger.LogWarning("AddPlayerAsync returned {success} for player {UserName}", success, string.Concat(playerDto.FirstName, " ", playerDto.LastName));
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Player create successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating player {UserName}", string.Concat(playerDto.FirstName, " ", playerDto.LastName));
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.OpenCreateModal = true;
                viewModel.PlayerDetailDtos = await repository.GetAllPlayerAsync();
                return View(nameof(Index), viewModel);
            }
        }

        // POST: PlayerController/Update
        [HttpPost("update/{playerId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromForm] PlayerDto playerDto, [FromRoute] int playerId)
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

                if (playerDto.PhotoFile is not null || playerDto.PhotoFile?.Length > 0)
                {
                    var validation = FileValidator.Validate(playerDto.PhotoFile);
                    if (!validation.IsValid)
                    {
                        ModelState.AddModelError(nameof(playerDto.PhotoFile), validation.ErrorMessage ?? "Invalid file");
                        return RedirectToAction(nameof(Index));
                    }
                }

                if (!await repository.PlayerExistingByClubAsync(playerDto, playerId)) throw new Exception($"This player {playerId} is already registered in this club.");

                var success = await repository.UpdatePlayerAsync(playerDto);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to update player. Please try again or contact admin.");
                    _logger.LogWarning("UpdatePlayerAsync returned {success} for player {UserName}", success, string.Concat(playerDto.FirstName, " ", playerDto.LastName));
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Player update successfully";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating player {UserName}", string.Concat(playerDto.FirstName, " ", playerDto.LastName));
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.OpenCreateModal = true;
                viewModel.PlayerDetailDtos = await repository.GetAllPlayerAsync();
                return View(nameof(Index), viewModel);
            }
        }

        // POST: PlayerController/Delete
        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int playerId)
        {
            try
            {
                var playerExisting = await repository.FindPlayerByIdAsync(playerId);
                if (playerExisting is null)
                {
                    ModelState.AddModelError(nameof(playerId), "A player id is not found.");
                    return RedirectToAction(nameof(Index));
                }

                var success = await repository.DeletePlayerAsync(playerId, playerExisting.Photo);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to update player. Please try again or contact admin.");
                    _logger.LogWarning("DeletePlayerAsync returned {success} for player {UserName}", success, string.Concat(playerExisting.FirstName, " ", playerExisting.LastName));
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Player delete successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting player {PlayerId}", playerId);
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.OpenCreateModal = true;
                viewModel.PlayerDetailDtos = await repository.GetAllPlayerAsync();
                return View(nameof(Index), viewModel);
            }
        }

        // POST: PlayerController/FindPlayerById
        [HttpPost("get-player/{playerId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindPlayerById([FromRoute] int playerId)
        {
            try
            {
                var playerDto = await repository.FindPlayerByIdAsync(playerId);
                if (playerDto is null)
                {
                    ModelState.AddModelError(nameof(playerId), "A player is not found.");
                    return RedirectToAction(nameof(Index));
                }

                return Json(playerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {PlayerId}", playerId);
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.PlayerDetailDtos = await repository.GetAllPlayerAsync();
                return View(nameof(Index), viewModel);
            }
        }
    }
}
