using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using BlackjackGame.Common;
using BlackjackGame.Model;
using BlackjackGame.Model;
using BlackjackGame.Service;
using BlackjackGame.WebSockets.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace BlackjackGame.WebSockets
{
    using BlackjackGame = Service.BlackjackGame;

    /// <summary>
    /// websocket handler implementation for blackjack game
    /// </summary>
    public class BlackjackGameWebSocketHandler : WebSocketHandler<BlackjackWebsocketWrapper>
    {
        private BlackjackGame _blackjackGame = new BlackjackGame();
        private BlackjackMessageBuilder _messageBuilder;

        /// <summary>
        /// intject dependency to use DbService
        /// </summary>
        private readonly Func<HttpContext, DbService> _dbService = context =>
            context.RequestServices.GetRequiredService<DbService>();


        public BlackjackGameWebSocketHandler()
        {
            _messageBuilder = new BlackjackMessageBuilder(_blackjackGame);
        }

        /// <inheritdoc />
        protected override BlackjackWebsocketWrapper WrapWebsocket(WebSocket webSocket)
        {
            return new BlackjackWebsocketWrapper()
            {
                WebSocket = webSocket
            };
        }

        /// <inheritdoc />
        public override string GenerateKey(HttpContext context)
        {
            return context.Session.Id;
        }

        /// <inheritdoc />
        /// <summary>
        /// Handle messages from websocket and response to all
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override async Task OnMessageReceive(HttpContext context, byte[] buffer, WebSocketReceiveResult result)
        {
            var message =
                JsonConvert.DeserializeObject<WebSocketTextMessage>(Encoding.ASCII.GetString(buffer)).Text;
            var messageType = message[BlackjackConstants.Message.MessageType] as string;
            switch (messageType)
            {
                case BlackjackConstants.Message.SitOnPlace:
                    await HandleNewUserInGame(message, context);
                    break;
                case BlackjackConstants.Message.AcceptGenerateCart:
                    await AcceptGenerateCart(context);
                    return;
                case BlackjackConstants.Message.DeclineGenerateCart:
                    await DeclineGenerateCart(context);
                    return;
            }
            await CheckStartGame();
        }

        /// <summary>
        /// handle user clicked accept cart
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task AcceptGenerateCart(HttpContext context)
        {
            var userInfo = Websockets[GenerateKey(context)];
            _blackjackGame.GenerateCartForUser(userInfo.Place);
            await SendGameInfoWithCurrentUser();
        }

        /// <summary>
        /// handle user clicked decline cart
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task DeclineGenerateCart(HttpContext context)
        {
            _blackjackGame.MoveNext(true);
            if (_blackjackGame.GameEnded)
            {
                await ProcessEndOfGame(context);
            }
            else
            {
                await SendGameInfoWithCurrentUser();
            }
        }

        /// <summary>
        /// handle end of game
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task ProcessEndOfGame(HttpContext context)
        {
            var gameResult = _blackjackGame.GameResult;
            await ReinitGame();
            await SendGameInfoWithWinner(context, gameResult);
        }

        /// <summary>
        /// reinitialize game to create new one
        /// </summary>
        /// <returns></returns>
        public async Task ReinitGame()
        {
            _blackjackGame = new BlackjackGame();
            _messageBuilder = new BlackjackMessageBuilder(_blackjackGame);
            foreach (var entry in Websockets.ToList())
            {
                var wrapper = entry.Value;
                var user = wrapper.BlackjackUser;
                if (user == null) continue;
                user = user.Reinit();
                _blackjackGame.AddUser(wrapper.Place, user);
            }
            await CheckStartGame(false);
        }

        /// <summary>
        /// Hnadle new user connected to game
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task HandleNewUserInGame(Dictionary<string, object> message, HttpContext context)
        {
            var place = Convert.ToInt32(message[BlackjackConstants.Message.PlaceNumber]);
            var name = context.GetUserName();
            var id = context.GetUserId();
            var curentUser = _blackjackGame.Users
                .FirstOrDefault(entry => entry.Value?.Name == name);
            if (curentUser.Value == null)
            {
                var user = new BlackjackUser(name, id);
                if (_blackjackGame.GameStarted)
                {
                    _blackjackGame.UsersInQueue.Add(place, user);
                }
                else
                {
                    _blackjackGame.AddUser(place, user);
                }
                Websockets[GenerateKey(context)].BlackjackUser = user;
                Websockets[GenerateKey(context)].Place = place;
            }
            else
            {
                Websockets[GenerateKey(context)].BlackjackUser = curentUser.Value;
                Websockets[GenerateKey(context)].Place = curentUser.Key;
            }
            await SendGameInfoWithCurrentUser();
        }

        public async Task SendGameInfoWithCurrentUser()
        {
            await SendGameInfoWithTask(_messageBuilder.BuildGameStateMessageWithCurrentUser);
        }

        public async Task SendGameInfoWithWinner(HttpContext context, BlackjackGameResult result)
        {
            await _dbService(context).SaveBlackjackGameResult(result);
            foreach (var websocketWrapper in Websockets)
            {
                var wrapper = websocketWrapper.Value;
                await SendMessageJson(wrapper.WebSocket,
                    _messageBuilder.BuildMessageWithWinner(wrapper.BlackjackUser, wrapper.Place, result));
            }
        }

        public async Task SendGameInfoWithTask(Func<BlackjackUser, int, Dictionary<string, object>> t)
        {
            var socketsToRemove = new List<string>();
            foreach (var websocketWrapper in Websockets.ToList())
            {
                var wrapper = websocketWrapper.Value;
                try
                {
                    await SendMessageJson(wrapper.WebSocket,
                        t(wrapper.BlackjackUser, wrapper.Place));
                }
                catch (Exception e)
                {
                    socketsToRemove.Add(websocketWrapper.Key);
                }
            }
            foreach (var key in socketsToRemove)
            {
                Websockets.Remove(key);
            }
        }

        public async Task CheckStartGame(bool sendInfo = true)
        {
            if (_blackjackGame.Users.Where(us => us.Value != null).ToList().Count > 1)
            {
                if (_blackjackGame.StartGame())
                {
                    await Generate2CartsForAllUsers(sendInfo);
                }
            }
        }


        public async Task Generate2CartsForAllUsers(bool sendInfo)
        {
            var places = Websockets.Where(w => w.Value.BlackjackUser != null).Select(w => w.Value.Place);
            foreach (var place in places)
            {
                _blackjackGame.GenerateCartForUser(place);
                _blackjackGame.GenerateCartForUser(place);
                _blackjackGame.MoveNext();
            }
            if (sendInfo)
            {
                await SendGameInfoWithCurrentUser();
            }
        }

        private async Task HandleUserConnectedToRoom(HttpContext context)
        {
            await SendGameInfoWithCurrentUser();
        }

        public override async Task OnClosedConnection(HttpContext context)
        {
            if (Websockets.ContainsKey(GenerateKey(context)))
            {
                var wrapper = Websockets[GenerateKey(context)];
                var place = wrapper.Place;
                wrapper.BlackjackUser = null;
                _blackjackGame.RemoveUser(place);
                await SendGameInfoWithCurrentUser();
                if (_blackjackGame.UsersInQueue.ContainsKey(place))
                {
                    _blackjackGame.UsersInQueue.Remove(place);
                }
                if (_blackjackGame.GameStarted && _blackjackGame.NotNullUsers.Count < 2)
                {
                    _blackjackGame.GameEnded = true;
                    await ProcessEndOfGame(context);
                }
            }
        }

        public override async Task ConnectionEstablish(HttpContext context)
        {
            await HandleUserConnectedToRoom(context);
        }

        protected static Dictionary<string, object> Dictionary()
        {
            return new Dictionary<string, object>();
        }
    }
}