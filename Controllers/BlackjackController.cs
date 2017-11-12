using BlackjackGame.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlackjackGame.Controllers
{
    public class BlackjackController : Controller
    {
        [HttpGet]
        [Route("/test/blackjack/game")]
        public ActionResult GenerateGameAndTestIt()
        {
            var game = new Service.BlackjackGame();
            game.AddUser(0, new BlackjackUser("Igor"));
            game.AddUser(2, new BlackjackUser("Christina"));
            game.AddUser(1, new BlackjackUser("Alex"));
            game.GenerateCartForUser(0);
            game.GenerateCartForUser(0);
            game.GenerateCartForUser(0);
            game.GenerateCartForUser(1);
            game.GenerateCartForUser(1);
            game.GenerateCartForUser(1);
            game.GenerateCartForUser(2);
            game.GenerateCartForUser(2);
            game.GenerateCartForUser(2);
            return Json(game);
        }
        
    }
}