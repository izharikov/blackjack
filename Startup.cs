using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlackjackGame.Context;
using BlackjackGame.Extenstions;
using BlackjackGame.Model;
using BlackjackGame.Service;
using BlackjackGame.WebSockets;
using BlackjackGame.WebSockets.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlackjackGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentificationWithCookie();
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
            });
            var connectionString =
                Configuration.GetConnectionString("Database");
            services.AddDbContext<BlackjackGameContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<DbService, DbService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseWebSockets();
            app
                .AddWebSocketHandler("/ws/blackjack", new BlackjackGameWebSocketHandler())
                .AppplyWebSocketMiddleware();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new {controller = "Home", action = "Index"});
            });
        }
    }
}