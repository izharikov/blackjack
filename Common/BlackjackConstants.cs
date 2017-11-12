using System.Data;

namespace BlackjackGame.Common
{
    public static class BlackjackConstants
    {
        public static class Message
        {
            public const string MessageType = "messageType";
            public const string NewUserMessage = "newUser";
            public const string GameState = "gameState";
            public const string GenerateCard = "generateCard";
            public const string NoMoreCards = "noMoreCards";
            public const string SitOnPlace = "sitOnPlace";
            public const string PlaceNumber = "place";
            public const string PlayerName = "name";
            public const string GameStart = "gameStart";

            public const string AcceptGenerateCart = "acceptCart";
            public const string DeclineGenerateCart = "declineCart";
            public const string UserCards = "userCards";
        }
    }
}