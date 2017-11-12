using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using BlackjackGame.Model;
using Microsoft.AspNetCore.Builder;

namespace BlackjackGame.WebSockets.Common
{
    public static class WebSocketMiddleware
    {
        private static readonly Dictionary<string, IWebSocketHandler> _handlers =
            new Dictionary<string, IWebSocketHandler>();

        public static IApplicationBuilder AddWebSocketHandler(this IApplicationBuilder app, string path,
            IWebSocketHandler handler)
        {
            _handlers.Add(path, handler);
            return app;
        }

        public static void AppplyWebSocketMiddleware(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/ws"))
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var handler = _handlers[context.Request.Path];
                        var websocketKey = await handler.AddNewWebSocket(context);
                        await handler.InitializeWebSocket(context, websocketKey);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }
    }
}