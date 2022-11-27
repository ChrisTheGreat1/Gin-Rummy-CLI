using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11242022_Gin_Rummy.Helpers
{
    public static class HandMethods
    {
        public static List<Card> SortHand(List<Card> hand)
        {
            var sortedHand = hand.OrderBy(c => c.Suit).ThenBy(c => c.Rank).ToList();
            return sortedHand;
        }

        public static void DetectMelds(List<Card> hand)
        {
            List<List<Card>> cardsBySuit = new List<List<Card>>();
            List<List<Card>> melds = new List<List<Card>>();

            cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Spades).ToList());
            cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Clubs).ToList());
            cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Hearts).ToList());
            cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Diamonds).ToList());

            foreach (var cards in cardsBySuit)
            {
                if (cards.Count() >= 3)
                {
                    List<int> ranks = new List<int>();

                    foreach (var card in cards)
                    {
                        ranks.Add((int)card.Rank);
                    }

                    //List<int> ranks = new List<int>(new int[] {4, 5, 7, 8});
                    //bool isInOrder;

                    if (ranks.Count > 3)
                    {
                        while (ranks.Count > 3)
                        {
                            if (ranks[ranks.Count - 1] != (ranks[ranks.Count - 2] + 1) ||
                            ranks[ranks.Count - 1] != (ranks[ranks.Count - 3] + 2))
                            {
                                ranks.Remove(ranks.Last());
                                cards.Remove(cards.Last());
                            }

                            if (ranks.Count < 3) break;

                            if (ranks[0] != (ranks[1] - 1) ||
                                ranks[0] != (ranks[2] - 2))
                            {
                                ranks.Remove(ranks.First());
                                cards.Remove(cards.First());
                            }

                            if (ranks.Count < 3) break;

                            if (ranks.SequenceEqual(Enumerable.Range(ranks.First(), ranks.Count())))
                            {
                                melds.Add(cards);
                                break;
                            };
                        }
                    }

                    else
                    {
                        if (ranks.SequenceEqual(Enumerable.Range(ranks.First(), ranks.Count())))
                        {
                            melds.Add(cards);
                        };
                    }
                }
            }
        }


    }
}
