using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BlackjackGame.Model
{
    public class BlackjackUser
    {
        public string UserId { get; set; }
        
        public string Name { get; set; }

        [JsonIgnore]
        public int Score => Carts.Select(cart => cart.Rank.Value()).Sum();

        [JsonIgnore]
        public List<PlayCard> Carts { get; set; }

        public int CountOfCards => Carts.Count;

        public BlackjackUser(string name, string id)
        {
            Name = name;
            UserId = id;
            Carts = new List<PlayCard>();
        }

        public BlackjackUser Reinit()
        {
            Carts = new List<PlayCard>();
            return this;
        }

        public BlackjackUser Copy()
        {
            return new BlackjackUser(Name, UserId)
            {
                Carts = Carts
            };
        }
    }
}