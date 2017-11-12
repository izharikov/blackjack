using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackGame.Model;
using BlackjackGame.Models;
using Newtonsoft.Json;

namespace BlackjackGame.Service
{
    public class BlackjackGame
    {
        public bool GameStarted = false;
        public Dictionary<int, BlackjackUser> Users { get; set; }
        public Dictionary<int, BlackjackUser> UsersInQueue { get; set; }
        public Dictionary<int, BlackjackUser> NotNullUsers { get; set; }
        
        [JsonIgnore]
        public PlayCardSet PlayCardSet { get; set; }

        public BlackjackUser Banker { get; set; }
        public BlackjackUser CurrentUser { get; set; }
        public int CurrentUserIndex { get; set; }
        public int BankAmount { get; set; }
        public bool GameEnded { get; set; }

        public BlackjackGame()
        {
            Users = new Dictionary<int, BlackjackUser>();
            PlayCardSet = new PlayCardSet();
            UsersInQueue = new Dictionary<int, BlackjackUser>();
            foreach (var i in Enumerable.Range(0, 6))
            {
                Users.Add(i, null);
            }
        }

        public void AddUser(int place, BlackjackUser user)
        {
            Users[place] = user;
        }

        public void GenerateCartForUser(int place)
        {
            var user = Users[place];
            var cart = PlayCardSet.GenerateRandomCart();
            user.Carts.Add(cart);
        }

        public void RemoveUser(int place)
        {
            var removedUser = Users[place];
            Users[place] = null;
            NotNullUsers = new Dictionary<int, BlackjackUser>();
            var idx = 0;
            foreach (var user in Users.Values)
            {
                if (user != null)
                {
                    NotNullUsers.Add(idx, user);
                    idx++;
                }
            }
            if (removedUser == CurrentUser)
            {
                MoveNext(true);
            }
        }

        [JsonIgnore]
        public BlackjackGameResult GameResult =>
            new BlackjackGameResult()
            {
                Winners = Users.Values.Where(user => user != null && user.Score < 22)
                    .Select(user => user.Copy())
                    .OrderBy(user => Math.Abs(user.Score - 21)).ToList()
            };

        public bool StartGame()
        {
            if (GameStarted)
            {
                return false;
            }
            GameStarted = true;
            NotNullUsers = new Dictionary<int, BlackjackUser>();
            var idx = 0;
            foreach (var user in Users)
            {
                if (user.Value != null)
                {
                    NotNullUsers.Add(idx, user.Value);
                    idx++;
                }
            }
            var rand = new Random();
            var index = rand.Next(NotNullUsers.Count);
            Banker = NotNullUsers[index];
            CurrentUser = NotNullUsers[index];
            CurrentUserIndex = index;
            return true;
        }

        public void MoveNext(bool couldEndGame = false)
        {
            if (NotNullUsers.Count < 2)
            {
                GameEnded = true;
                return;
            }
            CurrentUserIndex = (CurrentUserIndex + 1) % NotNullUsers.Count;
            CurrentUser = NotNullUsers[CurrentUserIndex];
            if (couldEndGame && CurrentUser == Banker)
            {
                GameEnded = true;
            }
        }

        public void SetBankAmount(int amount)
        {
            BankAmount = amount;
        }
    }

    public class BlackjackGameResult
    {
        public List<BlackjackUser> Winners { get; set; }
    }
}