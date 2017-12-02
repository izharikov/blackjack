using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlackjackGame.Common;
using BlackjackGame.Context;
using BlackjackGame.Model;
using BlackjackGame.Model.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace BlackjackGame.Controllers
{
    [Route("/api")]
    public class UserController : Controller
    {
        private BlackjackGameContext _context;

        public UserController(BlackjackGameContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("all-users")]
        public ActionResult GetAllUsers()
        {
            return Json(_context.Users);
        }

        [Authorize]
        [HttpGet]
        [Route("user")]
        public JsonResult GetCurrentUserInfo()
        {
            var user = new { name = HttpContext.GetUserName() };
            return Json(user);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!CheckUserExistsAndSetId(loginModel))
            {
                return Json(SignInResult.Failed);
            }
            await LoginUserInSystem(loginModel);
            return Json(SignInResult.Success);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] LoginModel loginModel)
        {
            if (CheckUserExistsAndSetId(loginModel))
            {
                return Json(SignInResult.Failed);
            }
            var userId = Guid.NewGuid().ToString();
            _context.Users.Add(new User()
            {
                UserName = loginModel.Name,
                UserId = userId
            });
            loginModel.Id = userId;
            _context.SaveChanges();
            await Login(loginModel);
            return Json(SignInResult.Success);
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(SignInResult.Success);
        }

        private bool CheckUserExistsAndSetId(LoginModel loginModel)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == loginModel.Name);
            if (user != null)
            {
                loginModel.Id = user.UserId;
            }
            return user != null;
        }

        private async Task LoginUserInSystem(LoginModel loginModel)
        {
            var name = loginModel.Name;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim("id", loginModel.Id)
            };

            var userIdentity = new ClaimsIdentity(claims, "login");
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(userIdentity));
        }

        [HttpGet]
        [Authorize]
        [Route("/protected")]
        public ActionResult Protected()
        {
            return GetAllUsers();
        }

        [HttpGet]
        [Route("all-game-results")]
        public ActionResult GameResults()
        {
            var res = _context.GameResults
                .Include(gr => gr.UserGameResults)
                .ThenInclude(gr => gr.User)
                .ToList();
            var first = res.FirstOrDefault();
            var gameResults = first.UserGameResults;
            if (gameResults != null)
            {
                var game = gameResults;
            }
            return Json(res);
        }

        [HttpGet]
        [Route("game-res-for-user")]
        [Authorize]
        public ActionResult GameResForUsers()
        {
            var userId = HttpContext.GetUserId();
            var res = _context.Users
                .Where(u => u.UserId == userId)
                .Include(gr => gr.UserGameResults);
            var jsonRes = res.Select(user => new UserStatisticsJsonViewModel()
            {
                UserId = userId,
                GameResult = user.UserGameResults
                    .Where(ugr => ugr.GameResult != null)
                    .Select(ugr => new UserGameResultJsonViewModel()
                    {
                        GameId = ugr.GameResultId,
                        WinnerScore = ugr.GameResult.WinnerScore,
                        IsWinner = ugr.IsWinner,

                    }).ToList()
            }).FirstOrDefault();
            return Json(jsonRes);
        }
    }
}