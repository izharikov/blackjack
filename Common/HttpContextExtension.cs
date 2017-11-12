using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BlackjackGame.Common
{
    public static class HttpContextExtension
    {
        public static string GetUserName(this HttpContext context)
        {
            return context.User.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault();
        }
    }
}