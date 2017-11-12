using System;
using System.Threading.Tasks;
using BlackjackGame.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlackjackGame.Controllers
{
    public class BaseController : Controller
    {
        private readonly BlackjackGameContext _context;
        private readonly ILogger _logger;

        public BaseController(BlackjackGameContext context, ILogger<BaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("/api/inventories")]
        public async Task<JsonResult> ViewAllFromDatabase()
        {
            _logger.LogInformation("viewing all from db");
            return Json(await _context.Inventory.ToListAsync());
        }

        [HttpGet]
        [Route("/base/test")]
        public ActionResult TestaActionResult()
        {
            return Json(new Tuple<string, string>("1", "2"));
        }
    }
}