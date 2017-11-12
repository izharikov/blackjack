using System;
using System.Collections.Generic;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

namespace BlackjackGame.Model
{
    public class PlayCardSet
    {

        public List<PlayCard> ActiveCarts { get; set; }
        public List<PlayCard> UsedCarts { get; set; }

        public PlayCardSet()
        {
            ActiveCarts = new List<PlayCard>();
            UsedCarts = new List<PlayCard>();
            Init();
        }

        public PlayCard GenerateRandomCart()
        {
            var rand = new Random();
            var index = rand.Next(ActiveCarts.Count);
            var cart = ActiveCarts[index];
            ActiveCarts.RemoveAt(index);
            UsedCarts.Add(cart);
            return cart;
        }

        private void Init()
        {
            var rankCount = Enum.GetNames(typeof(Rank)).Length;
            var suitCount = Enum.GetNames(typeof(Suit)).Length;
            for (int i = 0; i < rankCount * suitCount; i++)
            {
                ActiveCarts.Add(new PlayCard()
                {
                    Rank = (Rank) Enum.GetValues(typeof(Rank)).GetValue(i % rankCount),
                    Suit = (Suit) Enum.GetValues(typeof(Suit)).GetValue(i % suitCount)
                });
            }
        }
    }
}