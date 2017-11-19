using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlackjackGame.Common;
using BlackjackGame.Context;
using BlackjackGame.Models;
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
            var user = new {name = HttpContext.GetUserName()};
            return Json(user);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!UserExists(loginModel))
            {
                return Json(SignInResult.Failed);
            }
            await Login(loginModel.Name);
            return Json(SignInResult.Success);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] LoginModel loginModel)
        {
            if (UserExists(loginModel))
            {
                return Json(SignInResult.Failed);
            }
            _context.Users.Add(new User()
            {
                UserName = loginModel.Name,
                Id = Guid.NewGuid().ToString()
            });
            _context.SaveChanges();
            await Login(loginModel.Name);
            return Json(SignInResult.Success);
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return Json(SignInResult.Success);
        }

        private bool UserExists(LoginModel loginModel)
        {
            return _context.Users.Any(u => u.UserName == loginModel.Name);
        }

        private async Task Login(string name)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name)
            };

            var userIdentity = new ClaimsIdentity(claims, "login");
            HttpContext.Session.SetString("name", name);
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
    }
}