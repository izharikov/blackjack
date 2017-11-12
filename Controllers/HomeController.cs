using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackjackGame.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View("Index");
        }
        
        [HttpGet]
        [Route("/login")]
        public ActionResult Login()
        {
            return View("Index");
        }
        
        [HttpGet]
        [Route("/register")]
        public ActionResult Register()
        {
            return View("Index");
        }
        
        [Authorize]
        [HttpGet]
        [Route("/gameroom")]
        public ActionResult GameRoom()
        {
            return View("Index");
        }
    }
}