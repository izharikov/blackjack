using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using BlackjackGame.Common;
using BlackjackGame.Model;
using BlackjackGame.Models;
using BlackjackGame.Service;
using BlackjackGame.WebSockets.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Newtonsoft.Json;


namespace BlackjackGame.WebSockets
{
    public class BlackjackGameWebSocketHandler : WebSocketHandler<BlackjackWebsocketWrapper>
    {
        private Service.BlackjackGame _blackjackGame = new Service.BlackjackGame();
        private BlackjackMessageBuilder _messageBuilder;

        public BlackjackGameWebSocketHandler()
        {
            _messageBuilder = new BlackjackMessageBuilder(_blackjackGame);
        }

        protected override BlackjackWebsocketWrapper WrapWebsocket(WebSocket webSocket)
        {
            return new BlackjackWebsocketWrapper()
            {
                WebSocket = webSocket
            };
        }

        public override string GenerateKey(HttpContext context)
        {
            return context.Session.Id;
        }

        public override async Task OnMessageReceive(HttpContext context, byte[] buffer, WebSocketReceiveResult result)
        {
            var message =
                JsonConvert.DeserializeObject<WebSocketTextMessage>(Encoding.ASCII.GetString(buffer)).Text;
            var messageType = message[BlackjackConstants.Message.MessageType] as string;
            switch (messageType)
            {
                case BlackjackConstants.Message.GenerateCard:
                    break;
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

        public async Task AcceptGenerateCart(HttpContext context)
        {
            var userInfo = _websockets[GenerateKey(context)];
            _blackjackGame.GenerateCartForUser(userInfo.Place);
            await SendGameInfoWithCurrentUser();
        }

        public async Task DeclineGenerateCart(HttpContext context)
        {
            _blackjackGame.MoveNext(true);
            if (_blackjackGame.GameEnded)
            {
                var gameResult = _blackjackGame.GameResult;
                await ReinitGame();
                await SendGameInfoWithWinner(gameResult);
            }
            else
            {
                await SendGameInfoWithCurrentUser();
            }
        }

        public async Task ReinitGame()
        {
            _blackjackGame = new Service.BlackjackGame();
            _messageBuilder = new BlackjackMessageBuilder(_blackjackGame);
            foreach (var entry in _websockets.ToList())
            {
                var wrapper = entry.Value;
                var user = wrapper.BlackjackUser;
                if (user != null)
                {
                    user = user.Reinit();
                    _blackjackGame.AddUser(wrapper.Place, user);
                }
            }
            await CheckStartGame(false);
        }

        public async Task HandleNewUserInGame(Dictionary<string, object> message, HttpContext context)
        {
            var place = Convert.ToInt32(message[BlackjackConstants.Message.PlaceNumber]);
            var name = context.GetUserName();
            var curentUser = _blackjackGame.Users
                .FirstOrDefault(entry => entry.Value?.Name == name);
            if (curentUser.Value == null)
            {
                var user = new BlackjackUser(name);
                if (_blackjackGame.GameStarted)
                {
                    _blackjackGame.UsersInQueue.Add(place, user);
                }
                else
                {
                    _blackjackGame.AddUser(place, user);
                }
                _websockets[GenerateKey(context)].BlackjackUser = user;
                _websockets[GenerateKey(context)].Place = place;
            }
            else
            {
                _websockets[GenerateKey(context)].BlackjackUser = curentUser.Value;
                _websockets[GenerateKey(context)].Place = curentUser.Key;
            }
            await SendGameInfoWithCurrentUser();
        }

        public async Task SendGameInfoWithCurrentUser()
        {
            await SendGameInfoWithTask(_messageBuilder.BuildGameStateMessageWithCurrentUser);
        }

        public async Task SendGameInfoWithWinner(BlackjackGameResult result)
        {
            foreach (var websocketWrapper in _websockets)
            {
                var wrapper = websocketWrapper.Value;
                await SendMessageJson(wrapper.WebSocket,
                    _messageBuilder.BuildMessageWithWinner(wrapper.BlackjackUser, wrapper.Place, result));
            }
        }

        public async Task SendGameInfoWithTask(Func<BlackjackUser, int, Dictionary<string, object>> t)
        {
            var socketsToRemove = new List<string>();
            foreach (var websocketWrapper in _websockets.ToList())
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
                _websockets.Remove(key);
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
            var places = _websockets.Where(w => w.Value.BlackjackUser != null).Select(w => w.Value.Place);
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
            // TODO : reset state of game
            if (_websockets.ContainsKey(GenerateKey(context)))
            {
                var wrapper = _websockets[GenerateKey(context)];
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
                    var gameResult = _blackjackGame.GameResult;
                    await ReinitGame();
                    await SendGameInfoWithWinner(gameResult);
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