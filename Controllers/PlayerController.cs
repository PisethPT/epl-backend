using epl_backend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace epl_backend.Controllers
{
    public class PlayerController : Controller
    {
        private readonly PlayerViewModel viewModel;
        public PlayerController()
        {
            viewModel = new PlayerViewModel();
        }

        // GET: PlayerController
        public ActionResult Index()
        {
            return View(viewModel);
        }

    }
}
