namespace BlackjackGame.Model
{
    public class PlayCard
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public string RankStr => Rank.ToString();
        public string SuitStr => Suit.ToString();
        public int Value => Rank.Value();
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
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public static class RankValue
    {
        public static int Value(this Rank rank)
        {
            switch (rank)
            {
                case Rank.Two:
                    return 2;
                case Rank.Three:
                    return 3;
                case Rank.Four:
                    return 4;
                case Rank.Five:
                    return 5;
                case Rank.Six:
                    return 6;
                case Rank.Seven:
                    return 7;
                case Rank.Eight:
                    return 8;
                case Rank.Nine:
                    return 9;
                case Rank.Ten:
                    return 10;
                case Rank.Jack:
                    return 2;
                case Rank.Queen:
                    return 3;
                case Rank.King:
                    return 4;
                case Rank.Ace:
                    return 11;
            }
            return -1;
        }
    }

//    public class Rank
//    {
//        public static Rank Two = new Rank(2);
//        public static Rank Three = new Rank(3);
//        public static Rank Four = new Rank(4);
//        public static Rank Five = new Rank(5);
//        public static Rank Six = new Rank(6);
//        public static Rank Seven = new Rank(7);
//        public static Rank Eight = new Rank(8);
//        public static Rank Nine = new Rank(9);
//        public static Rank Ten = new Rank(10);
//        public static Rank Jack = new Rank(2);
//        public static Rank Queen = new Rank(3);
//        public static Rank King = new Rank(4);
//        public static Rank Ace = new Rank(11);
//
//        public int Value { get; protected set; }
//
//        private Rank(int value)
//        {
//            Value = value;
//        }
//       
//    }
}