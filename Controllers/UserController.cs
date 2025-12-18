using System.Security.Claims;
using epl_backend.Data.Repositories.Interfaces;
using epl_backend.Helper;
using epl_backend.Models.DTOs;
using epl_backend.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace epl_backend.Controllers
{
    [Route("auth")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository repository;
        private UserViewModel viewModel;

        public UserController(ILogger<UserController> logger, IUserRepository repository)
        {
            _logger = logger;
            viewModel = new UserViewModel();
            this.repository = repository;
        }

        [Authorize]
        [HttpGet("all-user")]
        public async Task<IActionResult> Index()
        {
            viewModel.userDtos = await repository.GetAllUsersAsync();
            return View(viewModel);
        }

        // GET: UserController
        [HttpGet("login")]
        public ActionResult Login() => View(viewModel);

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await repository.FindByEmailAsync(userLoginDto.Email);
            if (user is not null && await repository.CheckPasswordAsync(user, userLoginDto.Password))
            {
                var roleDto = await repository.GetRolesAsync(user);

                // Build claims identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId!),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                foreach (var role in roleDto.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in (issue auth cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = userLoginDto.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });
                return RedirectToAction("Dashboard", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(viewModel);
        }

        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("access-denied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            // Optionally pass data to the view (e.g., reason, return URL)
            ViewData["Title"] = "404 Access Denied";
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var validation = FileValidator.Validate(userDto.PhotoFile);
            if (!validation.IsValid)
            {
                ModelState.AddModelError(nameof(userDto.PhotoFile), validation.ErrorMessage ?? "Invalid file");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (await repository.FindByEmailAsync(userDto.Email) is not null)
                {
                    ModelState.AddModelError(nameof(userDto.Email), "A user with this email already exists.");
                    return RedirectToAction(nameof(Index));
                }

                var success = await repository.AddUserAsync(userDto);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to create user. Please try again or contact admin.");
                    _logger.LogWarning("AddUserAsync returned {success} for user {UserName}", success, string.Concat(userDto.FirstName, " ", userDto.LastName));
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "User create successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Create canceled for user {UserName}", string.Concat(userDto.FirstName, " ", userDto.LastName));
                ModelState.AddModelError(string.Empty, "Request was cancelled.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserName}", string.Concat(userDto.FirstName, " ", userDto.LastName));
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.OpenCreateModal = true;
                viewModel.userDtos = await repository.GetAllUsersAsync();
                return View(nameof(Index), viewModel);
            }
        }

        [HttpPost("get-user/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindUserByUserId([FromRoute] string userId)
        {
            try
            {
                var userDto = await repository.FindUserByIdAsync(userId);
                if (userDto is null)
                {
                    ModelState.AddModelError(nameof(userId), "A user is not found.");
                    return RedirectToAction(nameof(Index));
                }
                return Json(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", userId);
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.userDtos = await repository.GetAllUsersAsync();
                return View(nameof(Index), viewModel);
            }
        }

        [HttpPost("update/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromForm] UserDto userDto, [FromRoute] string userId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction(nameof(Index));

                if (userDto.PhotoFile is not null || userDto.PhotoFile?.Length > 0)
                {
                    var validation = FileValidator.Validate(userDto.PhotoFile);
                    if (!validation.IsValid)
                    {
                        ModelState.AddModelError(nameof(userDto.PhotoFile), validation.ErrorMessage ?? "Invalid file");
                        return RedirectToAction(nameof(Index));
                    }
                }

                if (!await repository.UserByExistEmail(userDto.Email, userId))
                {
                    ModelState.AddModelError(nameof(userDto.Email), "A user with this email already exists.");
                    return RedirectToAction(nameof(Index));
                }

                var success = await repository.UpdateUserAsync(userDto);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to update user. Please try again or contact admin.");
                    _logger.LogWarning("UpdateUserAsync returned {success} for user {UserName}", success, string.Concat(userDto.FirstName, " ", userDto.LastName));
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "User update successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Update canceled for user {UserName}", string.Concat(userDto.FirstName, " ", userDto.LastName));
                ModelState.AddModelError(string.Empty, "Request was cancelled.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserName}", string.Concat(userDto.FirstName, " ", userDto.LastName));
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.OpenCreateModal = true;
                viewModel.userDtos = await repository.GetAllUsersAsync();
                return View(nameof(Index), viewModel);
            }
        }

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            try
            {
                var existingUser = await repository.FindUserByIdAsync(userId);
                if (existingUser is null)
                {
                    ModelState.AddModelError(nameof(userId), "A user id is not found.");
                    return RedirectToAction(nameof(Index));
                }

                var success = await repository.DeleteUserAsync(userId, existingUser.Photo);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to delete user. Please try again or contact admin.");
                    _logger.LogWarning("DeleteUserAsync returned {Success} for club {UserName}", success, string.Concat(existingUser.FirstName, existingUser.LastName));
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "User delete successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delete user {UserId}", userId);
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.userDtos = await repository.GetAllUsersAsync();
                return View(nameof(Index), viewModel);
            }
        }
    }
}
