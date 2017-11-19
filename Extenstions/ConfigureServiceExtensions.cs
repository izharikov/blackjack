using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BlackjackGame.Extenstions
{
    public static class ConfigureServiceExtensions
    {
        public static IServiceCollection AddAuthentificationWithCookie(this IServiceCollection services)
        {
            services
                .AddAuthentication(auth =>
                {
                    auth.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = ctx =>
                        {
                            if (ctx.Request.Path.StartsWithSegments("/api"))
                            {
                                ctx.FillApiUnauthenticateResponse();
                                return Task.CompletedTask;
                            }
                            ctx.Response.Redirect(ctx.RedirectUri);
                            return Task.CompletedTask;
                        },
                    };
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                });
            return services;
        }

        private static void FillApiUnauthenticateResponse(this RedirectContext<CookieAuthenticationOptions> ctx)
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            ctx.Response.ContentType = "application/json";
            var response = Encoding.ASCII.GetBytes("{\"error\" : \"anauthorized\"}");
            ctx.Response.Body.Write(response, 0, response.Length); 
        }
    }
}