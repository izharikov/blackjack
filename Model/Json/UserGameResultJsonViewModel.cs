namespace BlackjackGame.Model.Json
{
    public class UserGameResultJsonViewModel
    {
        public string GameId { get; set; }

        public bool IsWinner { get; set; }

        public int WinnerScore { get; set; }
    }
}