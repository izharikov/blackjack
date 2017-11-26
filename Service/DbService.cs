using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BlackjackGame.Context;
using BlackjackGame.Model;

namespace BlackjackGame.Service
{
    public class DbService
    {
        private BlackjackGameContext _blackjackGameContext;

        public DbService(BlackjackGameContext blackjackGameContext)
        {
            _blackjackGameContext = blackjackGameContext;
        }

        public async Task SaveBlackjackGameResult(BlackjackGameResult result)
        {
            var winnerIds = result.Winners.Select(us => us.UserId);
            var allUserInGameIds = result.AllUsers.Select(us => us.UserId);
            var allUsers = _blackjackGameContext.Users.Where(u => allUserInGameIds.Contains(u.UserId));
            var gameResult = new GameResult()
            {
                GameResultId = Guid.NewGuid().ToString(),
                WinnerScore = result.Sum
            };
            allUsers.ToList().ForEach(user =>
            {
                var userGameResult = new UserGameResult
                {
                    User = user,
                    GameResult = gameResult,
                    IsWinner = winnerIds.Contains(user.UserId)
                };
                if (user.UserGameResults == null)
                {
                    user.UserGameResults = new Collection<UserGameResult>();
                }
                user.UserGameResults.Add(userGameResult);
            });
            _blackjackGameContext.GameResults.Add(gameResult);
            await _blackjackGameContext.SaveChangesAsync();
        }
    }
}