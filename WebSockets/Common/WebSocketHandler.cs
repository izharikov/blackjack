using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlackjackGame.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Newtonsoft.Json;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation.PredefinedTransformations;

namespace BlackjackGame.WebSockets.Common
{
    public abstract class WebSocketHandler<TWebsocketWrapper> : IWebSocketHandler
        where TWebsocketWrapper : WebscoketWrapper
    {
        protected readonly Dictionary<string, TWebsocketWrapper> _websockets;

        public WebSocketHandler()
        {
            _websockets = new Dictionary<string, TWebsocketWrapper>();
        }

        public abstract Task OnMessageReceive(HttpContext context, byte[] buffer, WebSocketReceiveResult result);

        protected WebSocket GetWebsocket(string key)
        {
            return _websockets[key].WebSocket;
        }

        protected abstract TWebsocketWrapper WrapWebsocket(WebSocket webSocket);

        public async Task<string> AddNewWebSocket(HttpContext context)
        {
            var key = GenerateKey(context);
            var websocket = await context.WebSockets.AcceptWebSocketAsync();
            _websockets.Add(key, WrapWebsocket(websocket));
            return key;
        }

        public virtual string GenerateKey(HttpContext context)
        {
            return Guid.NewGuid().ToString();
        }

        public virtual async Task ConnectionEstablish(HttpContext context)
        {
        }

        public async Task InitializeWebSocket(HttpContext context, string key)
        {
            var webSocket = GetWebsocket(key);
            var buffer = new byte[1024 * 8];
            await ConnectionEstablish(context);
            WebSocketReceiveResult result =
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await OnMessageReceive(context, buffer.Take(result.Count).ToArray(), result);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await OnClosedConnection(context);
            _websockets.Remove(key);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public virtual async Task OnClosedConnection(HttpContext context)
        {
        }

        public async Task SendToAll(byte[] buffer, WebSocketReceiveResult result)
        {
            foreach (var websocket in _websockets)
            {
                await SendMessage(websocket.Value.WebSocket, buffer, result);
            }
        }

        public async Task SendToAllJson(object obj)
        {
            foreach (var websocket in _websockets)
            {
                await SendMessageJson(websocket.Value.WebSocket, obj);
            }
        }

        public async Task SendToAllExceptSelf(object obj, HttpContext context)
        {
            await SendToAllExcept(obj, context.Session.Id);
        }

        public async Task SendToAllExcept(object obj, string key)
        {
            foreach (var websocket in _websockets)
            {
                if (websocket.Key != key)
                {
                    await SendMessageJson(websocket.Value.WebSocket, obj);
                }
            }
        }

        public async Task SendMessageJson(WebSocket receiver, object message)
        {
            var buffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
            await receiver.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
                WebSocketMessageType.Text,
                true, CancellationToken.None);
        }

        public async Task SendMessageJson(string id, object message)
        {
            var buffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
            await GetWebsocket(id).SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
                WebSocketMessageType.Text,
                true, CancellationToken.None);
        }

        public async Task SendToAllString(string message)
        {
            foreach (var websocket in _websockets)
            {
                await SendMessageString(websocket.Value.WebSocket, message);
            }
        }

        public async Task SendMessage(string id, byte[] buffer, WebSocketReceiveResult result)
        {
            var receiver = GetWebsocket(id);
            await SendMessage(receiver, buffer, result);
        }

        public async Task SendMessage(WebSocket receiver, byte[] buffer, WebSocketReceiveResult result)
        {
            await receiver.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count),
                result.MessageType,
                result.EndOfMessage, CancellationToken.None);
        }

        public async Task SendMessageString(WebSocket receiver, string message)
        {
            var buffer = Encoding.ASCII.GetBytes(message);
            await receiver.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
                WebSocketMessageType.Text,
                true, CancellationToken.None);
        }
    }

    public interface IWebSocketHandler
    {
        Task<string> AddNewWebSocket(HttpContext context);
        Task InitializeWebSocket(HttpContext context, string key);
    }
}