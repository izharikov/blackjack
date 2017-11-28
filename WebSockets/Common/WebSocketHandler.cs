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
    /// <summary>
    /// base abstraction for websocket handler
    /// </summary>
    /// <typeparam name="TWebsocketWrapper"></typeparam>
    public abstract class WebSocketHandler<TWebsocketWrapper> : IWebSocketHandler
        where TWebsocketWrapper : WebscoketWrapper
    {
        /// <summary>
        /// Dictionary to keep all connected websockets
        /// </summary>
        protected readonly Dictionary<string, TWebsocketWrapper> Websockets;

        protected WebSocketHandler()
        {
            Websockets = new Dictionary<string, TWebsocketWrapper>();
        }

        /// <summary>
        /// Callback when message received from client
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public abstract Task OnMessageReceive(HttpContext context, byte[] buffer, WebSocketReceiveResult result);

        /// <summary>
        /// Get websocket by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private WebSocket GetWebsocket(string key)
        {
            return Websockets[key].WebSocket;
        }

        /// <summary>
        /// Function to wrap websocket. Wrapper used for store some additional data
        /// connected with websocket
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        protected abstract TWebsocketWrapper WrapWebsocket(WebSocket webSocket);

        /// <summary>
        /// Add new connected websocket to room
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> AddNewWebSocket(HttpContext context)
        {
            var key = GenerateKey(context);
            var websocket = await context.WebSockets.AcceptWebSocketAsync();
            Websockets.Add(key, WrapWebsocket(websocket));
            return key;
        }

        /// <summary>
        /// Generate unique key based on context for new connected websocket
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual string GenerateKey(HttpContext context)
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Callback for connection established websocket
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task ConnectionEstablish(HttpContext context)
        {
        }

        /// <summary>
        /// All logic for websocket interaction is here. 
        /// Base flow is: 
        ///  <list type="bullet">
        /// <item>
        /// <description>Created new websocket: <see cref="GetWebsocket"/></description>
        /// </item>
        /// <item>
        /// <description>Called <see cref="ConnectionEstablish"/></description>
        /// </item>
        /// <item>
        /// <description>Then wait for <see cref="System.Net.WebSockets.WebSocket.ReceiveAsync"/>, that reads from websocket</description>
        /// </item>
        /// <item>
        /// <description>Called <see cref="OnMessageReceive"/> to handle received message. 
        /// Then again called <see cref="System.Net.WebSockets.WebSocket.ReceiveAsync"/>.
        /// It called until websocket connection ended
        /// </description>
        /// </item>
        /// <item>
        /// <description>Call <see cref="OnClosedConnection"/> to handle closed connection.
        /// Then websocket removed from <see cref="Websockets"/>.
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task InitializeWebSocket(HttpContext context, string key)
        {
            var webSocket = GetWebsocket(key);
            var buffer = new byte[1024 * 8];
            await ConnectionEstablish(context);
            var result =
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await OnMessageReceive(context, buffer.Take(result.Count).ToArray(), result);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await OnClosedConnection(context);
            Websockets.Remove(key);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        /// <summary>
        /// Called after close connection
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task OnClosedConnection(HttpContext context)
        {
        }

        #region Send to all methods

        public async Task SendToAll(byte[] buffer, WebSocketReceiveResult result)
        {
            foreach (var websocket in Websockets)
            {
                await SendMessage(websocket.Value.WebSocket, buffer, result);
            }
        }

        public async Task SendToAllJson(object obj)
        {
            foreach (var websocket in Websockets)
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
            foreach (var websocket in Websockets)
            {
                if (websocket.Key != key)
                {
                    await SendMessageJson(websocket.Value.WebSocket, obj);
                }
            }
        }

        public async Task SendToAllString(string message)
        {
            foreach (var websocket in Websockets)
            {
                await SendMessageString(websocket.Value.WebSocket, message);
            }
        }

        #endregion

        #region Send messages methods

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

        #endregion
    }

    /// <summary>
    /// interface to use it in middleware
    /// </summary>
    public interface IWebSocketHandler
    {
        /// <summary>
        /// Handle add new websocket
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<string> AddNewWebSocket(HttpContext context);

        /// <summary>
        /// Init websocket connection
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task InitializeWebSocket(HttpContext context, string key);
    }
}