using _11242022_Gin_Rummy.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11242022_Gin_Rummy.Models
{
    public class Card
    {
        //public const char SPADES = '♠';
        //public const char CLUBS = '♣';
        //public const char HEARTS = '♥';
        //public const char DIAMONDS = '♦';

        public Suit Suit { get; set; }

        public Rank Rank { get; set; }

        public bool IsInMeld { get; set; } = false;

        public int MeldGroupIdentifier { get; set; } = -1;

        public bool IsMeld3or4ofKind { get; set; } = false;

        public override string ToString()
        {
            var sb = new StringBuilder();

            switch (Rank)
            {
                case Rank.Ace:
                    sb.Append('A');
                    break;
                case Rank.Deuce:
                    sb.Append('2');
                    break;
                case Rank.Three:
                    sb.Append('3');
                    break;
                case Rank.Four:
                    sb.Append('4');
                    break;
                case Rank.Five:
                    sb.Append('5');
                    break;
                case Rank.Six:
                    sb.Append('6');
                    break;
                case Rank.Seven:
                    sb.Append('7');
                    break;
                case Rank.Eight:
                    sb.Append('8');
                    break;
                case Rank.Nine:
                    sb.Append('9');
                    break;
                case Rank.Ten:
                    sb.Append('T');
                    break;
                case Rank.Jack:
                    sb.Append('J');
                    break;
                case Rank.Queen:
                    sb.Append('Q');
                    break;
                case Rank.King:
                    sb.Append('K');
                    break;
            }

            switch (Suit)
            {
                case Suit.Spades:
                    sb.Append('♠');
                    break;
                case Suit.Clubs:
                    sb.Append('♣');
                    break;
                case Suit.Hearts:
                    sb.Append('♥');
                    break;
                case Suit.Diamonds:
                    sb.Append('♦');
                    break;
            }

            return sb.ToString();
        }
    }
}
