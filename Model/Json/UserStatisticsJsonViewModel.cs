using System.Collections.Generic;

namespace BlackjackGame.Model.Json
{
    public class UserStatisticsJsonViewModel
    {
        public string UserName { get; set; }

        public string UserId { get; set; }
        public List<UserGameResultJsonViewModel> GameResult { get; set; }
    }
}