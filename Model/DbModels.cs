using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Newtonsoft.Json;

namespace BlackjackGame.Model
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        
        public ICollection<UserGameResult> UserGameResults { get; set; }
    }

    public class GameResult
    {
        public string GameResultId { get; set; }
        
        public int WinnerScore { get; set; }

        
        public ICollection<UserGameResult> UserGameResults { get; set; }
    }

    
    public class UserGameResult
    {
        
        public string UserId { get; set; }

        
        public string GameResultId { get; set; }

        
        public User User { get; set; }

        public GameResult GameResult { get; set; }
      
        public bool IsWinner { get; set; }
    }
}