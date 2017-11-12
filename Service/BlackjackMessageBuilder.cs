using System.Collections.Generic;
using BlackjackGame.Common;
using BlackjackGame.Model;

namespace BlackjackGame.Service
{
    public class BlackjackMessageBuilder
    {
        private BlackjackGame BlackjackGame;

        public BlackjackMessageBuilder(BlackjackGame blackjackGame)
        {
            BlackjackGame = blackjackGame;
        }

        public Dictionary<string, object> BuildGameStateMessage()
        {
            var messageToSend = Dictionary();
            messageToSend[BlackjackConstants.Message.MessageType] = BlackjackConstants.Message.GameState;
            messageToSend[BlackjackConstants.Message.GameState] = BlackjackGame;
            return messageToSend;
        }

        public Dictionary<string, object> BuildMessageWithWinner(BlackjackUser user, int place, BlackjackGameResult result)
        {
            var message = BuildGameStateMessageWithCurrentUser(user, place);
            message["winner"] = result;
            return message;
        }

        public Dictionary<string, object> BuildGameStateMessageWithCurrentUser(BlackjackUser user, int place)
        {
            var messageToSend = Dictionary();
            messageToSend[BlackjackConstants.Message.MessageType] = BlackjackConstants.Message.GameState;
            messageToSend[BlackjackConstants.Message.GameState] = BlackjackGame;
            if (user != null)
            {
                var currentUser = Dictionary();
                currentUser["carts"] = user.Carts;
                currentUser["place"] = place;
                currentUser["name"] = user.Name;
                messageToSend["currentUser"] = currentUser;
            }
            return messageToSend;
        }


        public Dictionary<string, object> BuildStartOfGameMessage()
        {
            var message = Dictionary();
            message[BlackjackConstants.Message.MessageType] = BlackjackConstants.Message.GameStart;
            message[BlackjackConstants.Message.GameState] = BlackjackGame;
            return message;
        }

        public Dictionary<string, object> BuildUserDetailsMessage(int place)
        {
            var message = Dictionary();
            message[BlackjackConstants.Message.MessageType] = BlackjackConstants.Message.GameStart;
            message[BlackjackConstants.Message.UserCards] = BlackjackGame.Users[place].Carts;
            return message;
        }

        protected static Dictionary<string, object> Dictionary()
        {
            return new Dictionary<string, object>();
        }
    }
}