using epl_backend.Models.DTOs;
using epl_backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace epl_backend.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private UserViewModel viewModel;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
            viewModel = new UserViewModel();
        }

        // GET: UserController
        [HttpGet]
        public ActionResult Login() => View(viewModel);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            return View(viewModel);
        }
    }
}
