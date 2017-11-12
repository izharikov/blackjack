namespace BlackjackGame.Model
{
    public class PlayCard
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public string RankStr => Rank.ToString();
        public string SuitStr => Suit.ToString();
    }

    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public enum Rank
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 2,
        Queen = 3,
        King = 4,
        Ace = 11
    }
}