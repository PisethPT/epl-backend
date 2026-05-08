using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Helper;
using PremierLeague_Backend.Helper.SqlCommands;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Models.SelectListItems;
using PremierLeague_Backend.Models.ViewModels;

namespace PremierLeague_Backend.Controllers
{
    [Authorize]
    [Route("en/videos")]
    public class VideoController : Controller
    {
        private readonly VideoViewModel viewModel;
        private readonly ILogger<VideoController> _logger;
        private readonly IVideoRepository repository;
        private readonly ISelectListItems selectListItems;

        public VideoController(ILogger<VideoController> logger, IVideoRepository repository, ISelectListItems selectListItems)
        {
            this.viewModel = new VideoViewModel();
            this._logger = logger;
            this.repository = repository;
            this.selectListItems = selectListItems;
        }

        // GET: VideoController
        public async Task<ActionResult> IndexAsync(int page = 1)
        {
            try
            {
                int pageSize = 20;
                viewModel.VideoDetailDtos = await repository.GetAllVideosAsync(page);
                viewModel.SelectListItemClubs = await selectListItems.SelectListItemClubAsync();
                viewModel.SelectListItemPlayers = await repository.GetAllPlayersAsync();
                viewModel.SelectListItemMatches = await selectListItems.SelectListItemMatchesAsync(SelectListItemCommands.SelectListItemMatchCommands);
                viewModel.selectListItemVideoTag = await selectListItems.SelectListItemsAsync("PL_CommandSelectListItemVideoTag");
                viewModel.SelectListItemSeason = await selectListItems.SelectListItemsAsync("PL_CommandSelectListItemSeason");

                int totalCount = await repository.GetVideoCountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading videos");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }
        }

        // POST: VideoController/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(VideoDto videoDto)
        {
            try
            {
                videoDto.Publisher = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "12c64674-5ea2-484d-90b1-7419243b1758"; // TODO: Get user id from session
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

                if (!await repository.FindVideoExisting(videoDto))
                {
                    ModelState.AddModelError(nameof(videoDto.Title), "A video with this title already exists.");
                    return RedirectToAction(nameof(Index));
                }

                var success = await repository.AddVideoAsync(videoDto);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to create video. Please try again or contact admin.");
                    _logger.LogWarning("AddVideoAsync returned {success} for video {VideoTitle}", success, videoDto.Title);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Video created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating video");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(nameof(Index), viewModel);
            }
        }

        // POST: VideoController/Update
        [HttpPost("update/{videoId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAsync([FromRoute] int videoId, [FromForm] VideoDto videoDto)
        {
            try
            {
                videoDto.Publisher = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "12c64674-5ea2-484d-90b1-7419243b1758"; // TODO: Get user id from session
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

                if (!await repository.FindVideoExisting(videoDto, videoId))
                {
                    ModelState.AddModelError(nameof(videoDto.Title), "A video with this title already exists.");
                    return RedirectToAction(nameof(Index));
                }

                var success = await repository.UpdateVideoAsync(videoId, videoDto);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to update video. Please try again or contact admin.");
                    _logger.LogWarning("UpdateVideoAsync returned {success} for video {VideoTitle}", success, videoDto.Title);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Video updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating video");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(nameof(Index), viewModel);
            }
        }

        // POST: VideoController/Delete
        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(int videoId)
        {
            try
            {
                if (videoId <= 0 || await repository.GetVideoByIdAsync(videoId) == null)
                {
                    ModelState.AddModelError(string.Empty, "Video not found.");
                    return RedirectToAction(nameof(Index));
                }

                if (await repository.GetVideoByIdAsync(videoId) is null)
                {
                    ModelState.AddModelError(string.Empty, $"Video by id {videoId} not found.");
                    return RedirectToAction(nameof(Index));
                }

                var success = await repository.DeleteVideoAsync(videoId);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to delete video. Please try again or contact admin.");
                    _logger.LogWarning("DeleteVideoAsync returned {success} for video {VideoId}", success, videoId);
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Video deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting video");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(nameof(Index), viewModel);
            }
        }

        // GET: VideoController/GetVideoById
        [HttpGet("get-video/{videoId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetVideoByIdAsync([FromRoute] int videoId)
        {
            try
            {
                var video = await repository.GetVideoByIdAsync(videoId);
                if (video is null)
                {
                    return Json(new
                    {
                        StatusCode = 404,
                        Message = $"Video by id {videoId} not found.",
                    });
                }
                return Json(new
                {
                    StatusCode = 200,
                    Message = "Commit Transaction Success.",
                    Data = new
                    {
                        VideoItem = video
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting video by id");
                return Json(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }

        // GET: VideoController/GetVideoById
        [HttpGet("get-video-category/{isTheArchive:bool}")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetVideoCategoriesAsync([FromRoute] bool isTheArchive)
        {
            try
            {
                var data = await selectListItems.SelectListItemAsync<SelectListItemHasSubtitle>
                (
                    SelectListItemCommands.CommandSelectListItemVideosCategories,
                    new Dictionary<string, string>
                {
                    {"@IsTheArchive", isTheArchive.ToString().ToLower() }
                },
                rdr => new SelectListItemHasSubtitle
                {
                    Value = rdr.SafeGetInt("Value"),
                    Label = rdr.SafeGetString("Label"),
                    Subtitle = rdr.SafeGetString("Subtitle")
                });
                return Json(new
                {
                    StatusCode = 200,
                    Message = "Commit Transaction Success.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting video by id");
                return Json(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }
    }
}
