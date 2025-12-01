using Microsoft.AspNetCore.Mvc;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Models.DTOs;
using epl_backend.Models.ViewModels;

namespace epl_backend.Controllers
{
    [Route("clubs")]
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly ILogger<ClubController> _logger;
        private ClubViewModel viewModel;
        public ClubController(IClubRepository clubRepository, ILogger<ClubController> logger)
        {
            _clubRepository = clubRepository;
            _logger = logger;
            this.viewModel = new ClubViewModel();
        }

        // GET: ClubController
        [HttpGet("all-club")]
        public async Task<ActionResult> Index()
        {
            viewModel.clubDtos = await _clubRepository.GetAllClubsAsync();
            viewModel.clubDto = new ClubDto();
            return View(viewModel);
        }

        // POST: ClubController/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ClubDto clubDto)
        {
            // basic server-side model check first
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            // Server-side file checks (mirror the checks in your repository if also applied there)
            if (clubDto.CrestFile != null && clubDto.CrestFile.Length > 0)
            {
                var allowedExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".svg" };
                var ext = Path.GetExtension(clubDto.CrestFile.FileName ?? string.Empty);
                const long maxBytes = 2 * 1024 * 1024; // 2 MB

                if (!allowedExt.Contains(ext))
                {
                    ModelState.AddModelError(nameof(clubDto.CrestFile), "Invalid crest file type. Allowed: PNG, JPG, SVG.");
                    return RedirectToAction(nameof(Index));
                }

                if (clubDto.CrestFile.Length > maxBytes)
                {
                    ModelState.AddModelError(nameof(clubDto.CrestFile), "Crest file is too large. Max allowed is 2 MB.");
                    return RedirectToAction(nameof(Index));
                }
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(clubDto.Name) && await _clubRepository.ExistsByNameAsync(clubDto.Name))
                {
                    ModelState.AddModelError(nameof(clubDto.Name), "A club with this name already exists.");
                    return RedirectToAction(nameof(Index));
                }

                var newId = await _clubRepository.AddClubAsync(clubDto);

                if (newId <= 0)
                {
                    // repository indicated failure — show friendly error
                    ModelState.AddModelError(string.Empty, "Unable to create club. Please try again or contact admin.");
                    _logger.LogWarning("AddClubAsync returned {NewId} for club {ClubName}", newId, clubDto.Name);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Club created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Create canceled for club {ClubName}", clubDto.Name);
                ModelState.AddModelError(string.Empty, "Request was cancelled.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating club {ClubName}", clubDto.Name);
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.OpenCreateModal = true;
                viewModel.clubDtos = await _clubRepository.GetAllClubsAsync();
                return View(nameof(Index), viewModel);
            }
        }

        // POST: ClubController/Delete
        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int clubId)
        {
            try
            {
                var existingClub = await _clubRepository.GetClubByIdAsync(clubId);
                if (existingClub is null)
                {
                    ModelState.AddModelError(nameof(existingClub.Name), "A club is not found.");
                    return RedirectToAction(nameof(Index));
                }

                var success = await _clubRepository.DeleteClubAsync(existingClub.Id);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to create club. Please try again or contact admin.");
                    _logger.LogWarning("DeleteClubAsync returned {NewId} for club {ClubName}", success, existingClub.Name);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Club created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delete club {ClubId}", clubId);
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.clubDtos = await _clubRepository.GetAllClubsAsync();
                return View(nameof(Index), viewModel);
            }
        }

        // POST: ClubController/Delete
        [HttpPost("get-club/{clubId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetClubById([FromRoute] int clubId)
        {
            try
            {
                var clubDto = await _clubRepository.GetClubByIdAsync(clubId);
                if (clubDto is null)
                {
                    ModelState.AddModelError(nameof(clubDto.Name), "A club is not found.");
                    return RedirectToAction(nameof(Index));
                }

                return Json(clubDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting club {ClubName}", clubId);
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.clubDtos = await _clubRepository.GetAllClubsAsync();
                return View(nameof(Index), viewModel);
            }
        }

        // POST: ClubController/Create
        [HttpPost("edit/{clubId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] ClubDto clubDto, [FromRoute] int clubId)
        {
            // basic server-side model check first
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            // Server-side file checks (mirror the checks in your repository if also applied there)
            if (clubDto.CrestFile != null && clubDto.CrestFile.Length > 0)
            {
                var allowedExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".svg" };
                var ext = Path.GetExtension(clubDto.CrestFile.FileName ?? string.Empty);
                const long maxBytes = 2 * 1024 * 1024; // 2 MB

                if (!allowedExt.Contains(ext))
                {
                    ModelState.AddModelError(nameof(clubDto.CrestFile), "Invalid crest file type. Allowed: PNG, JPG, SVG.");
                    return RedirectToAction(nameof(Index));
                }

                if (clubDto.CrestFile.Length > maxBytes)
                {
                    ModelState.AddModelError(nameof(clubDto.CrestFile), "Crest file is too large. Max allowed is 2 MB.");
                    return RedirectToAction(nameof(Index));
                }
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(clubDto.Name) && await _clubRepository.ExistsByNameAsync(clubDto.Name))
                {
                    ModelState.AddModelError(nameof(clubDto.Name), "A club with this name already exists.");
                    return RedirectToAction(nameof(Index));
                }

                var newId = await _clubRepository.AddClubAsync(clubDto);

                if (newId <= 0)
                {
                    // repository indicated failure — show friendly error
                    ModelState.AddModelError(string.Empty, "Unable to create club. Please try again or contact admin.");
                    _logger.LogWarning("AddClubAsync returned {NewId} for club {ClubName}", newId, clubDto.Name);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Club created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Create canceled for club {ClubName}", clubDto.Name);
                ModelState.AddModelError(string.Empty, "Request was cancelled.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating club {ClubName}", clubDto.Name);
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.OpenCreateModal = true;
                viewModel.clubDtos = await _clubRepository.GetAllClubsAsync();
                return View(nameof(Index), viewModel);
            }
        }

    }
}
