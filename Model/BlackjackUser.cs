using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BlackjackGame.Model
{
    public class BlackjackUser
    {
        public string Name { get; set; }

        [JsonIgnore]
        public int Score => Carts.Select(cart => cart.Rank.Value()).Sum();

        [JsonIgnore]
        public List<PlayCard> Carts { get; set; }

        public int CountOfCards => Carts.Count;

        public BlackjackUser(string name)
        {
            Name = name;
            Carts = new List<PlayCard>();
        }

        public BlackjackUser Reinit()
        {
            Carts = new List<PlayCard>();
            return this;
        }

        public BlackjackUser Copy()
        {
            return new BlackjackUser(Name)
            {
                Carts = Carts
            };
        }
    }
}