using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BlackjackGame.WebSockets
{
    public class WebsocketRoomMiddleware
    {
        public static readonly string WEBSOCKET_PREFIX = "/ws";

        private readonly Dictionary<string, WebSocket> websockets = new Dictionary<string, WebSocket>();

        private readonly Dictionary<string, Func<HttpContext, WebSocket, Task>> functions =
            new Dictionary<string, Func<HttpContext, WebSocket, Task>>();

        public WebsocketRoomMiddleware Add(string path, Func<HttpContext, WebSocket, Task> func)
        {
            functions.Add(path, func);
            return this;
        }

        public Func<HttpContext, Func<Task>, Task> Build()
        {
            return async (context, func) =>
            {
                var path = context.Request.Path.ToString();
                if (path.StartsWith(WEBSOCKET_PREFIX))
                {
                    var websocketPrefix = path.Substring(WEBSOCKET_PREFIX.Length);
                    WebSocket webSocket;
                    if (websockets.ContainsKey(websocketPrefix))
                    {
                        webSocket = websockets[websocketPrefix];
                    }
                    else
                    {
                        webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        websockets.Add(websocketPrefix, webSocket);
                    }
                    if (functions.ContainsKey(websocketPrefix))
                    {
                        await functions[websocketPrefix](context, webSocket);
                    }
                }
                else
                {
                    await func();
                }
            };
        }
    }
}