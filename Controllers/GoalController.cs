using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;

namespace PremierLeague_Backend.Controllers
{
    [Authorize]
    [Route("goals")]
    public class GoalController : Controller
    {
        private readonly ILogger<GoalController> _logger;
        private readonly IGoalRepository repository;

        public GoalController(ILogger<GoalController> logger, IGoalRepository repository)
        {
            this._logger = logger;
            this.repository = repository;
        }

        // GET: GoalController
        [HttpGet]
        public ActionResult Index(List<int> club, int season, int week = 0, int? page = 1)
        {
            return View();
        }

        // POST: GoalController/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromForm] GoalDto goalDto)
        {
            return View();
        }

        // POST: GoalController/Update/1
        [HttpPut("update/{goalId}")]
        [ValidateAntiForgeryToken]
        public IActionResult Update([FromRoute] int goalId, [FromForm] GoalDto goalDto)
        {
            return View();
        }

        // DELETE: GoalController/Delete/1
        [HttpDelete("delete/{goalId}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute] int goalId)
        {
            return View();
        }

        // GET: GoalController/Get-Goal/1
        [HttpGet("get-goal/{goalId}")]
        [ValidateAntiForgeryToken]
        public IActionResult GetGoalById([FromRoute] int goalId)
        {
            return View();
        }

    }
}
