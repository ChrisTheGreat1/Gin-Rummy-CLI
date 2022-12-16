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
        public static List<Card> NonKnockerCombinesUnmatchedCardsWithKnockersMelds(List<Card> handKnocker, List<Card> handNonKnocker)
        {
            // TODO: fix bug where knockers hands are taken out of meld upon combining
            List<Card> handOfMeldAndPotentialDiscards = handKnocker.Where(c => c.IsInMeld).ToList();
            handOfMeldAndPotentialDiscards.AddRange(handNonKnocker.Where(c => !c.IsInMeld).ToList());

            handOfMeldAndPotentialDiscards = DetectAndGroupByMelds(handOfMeldAndPotentialDiscards);

            foreach(var card in handOfMeldAndPotentialDiscards)
            {
                if(card.IsInMeld == true && handNonKnocker.Contains(card))
                {
                    handNonKnocker[handNonKnocker.IndexOf(card)].IsInMeld = true;
                }
            }

            return handNonKnocker;
        }

        /// <summary>
        /// Calculate the value of the hand that is passed in as parameter. Hand needs to have melds detected before being passed in.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns>Integer value of the hand.</returns>
        public static int CalculateHandValue(List<Card> hand)
        {
            int handValue = 0;

            foreach(var card in hand)
            {
                if(!card.IsInMeld)
                {
                    if(card.Rank == Rank.Jack || card.Rank == Rank.Queen || card.Rank == Rank.King) handValue += 10;
                    else handValue += (int)card.Rank;
                }
            }

            return handValue;
        }

        /// <summary>
        /// Determine if player can knock (ie. hand value is less than 10 points). Hand needs to have melds detected before being passed in.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns>Boolean denoting if hand is eligible to knock.</returns>
        public static bool CanPlayerKnock(List<Card> hand)
        {
            if (CalculateHandValue(hand) <= 10) return true;
            else return false;
        }

        public static bool DetectGin(List<Card> hand)
        {
            if (hand.All(c => c.IsInMeld)) return true;
            else return false;
        }

        public static List<Card> SortHand(List<Card> hand)
        {
            var sortedHand = hand.OrderBy(c => c.Suit).ThenBy(c => c.Rank).ToList();
            return sortedHand;
        }

        public static List<Card> SortHandWithMeldGroupings(List<Card> hand)
        {
            var sortedHand = hand.OrderBy(c => !c.IsInMeld).ThenBy(c => c.Rank).ToList(); 

            return sortedHand;
        }

        // TODO: refactor code duplication into methods
        // TODO: write documentation explaining the algorithm
        public static List<Card> DetectAndGroupByMelds(List<Card> hand)
        {
            foreach(var card in hand)
            {
                card.IsInMeld = false; // Reset all IsInMeld booleans in case player decided to break meld during discard
            }

            hand = SortHand(hand);

            #region Check for runs of three or more
            List<List<Card>> cardsBySuit = new List<List<Card>>();

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

                    //List<int> ranks = new List<int>(new int[] {1, 2, 3, 6, 10});

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
                                foreach(var card in cards)
                                {
                                        int findCardInHand = hand.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
                                        hand[findCardInHand].IsInMeld = true;
                                }

                                break;
                            };
                        }
                    }

                    else
                    {
                        if (ranks.SequenceEqual(Enumerable.Range(ranks.First(), ranks.Count())))
                        {
                            foreach (var card in cards)
                            {
                                    int findCardInHand = hand.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
                                    hand[findCardInHand].IsInMeld = true;
                            }
                        };
                    }
                }
            }
            #endregion

            #region Check for three/four of a kind
            var handNotInMeld = hand.Where(c => (c.IsInMeld == false));
            var groupByRank = handNotInMeld.GroupBy(c => c.Rank);

            foreach(var group in groupByRank)
            {

                if(group.Count() >= 3)
                {
                    foreach(var card in group)
                    {
                            int findCardInHand = hand.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
                            hand[findCardInHand].IsInMeld = true;
                    }
                }
            }
            #endregion

            return SortHandWithMeldGroupings(hand);
        }

        public static string HandToString(List<Card> hand)
        {
            var sb = new StringBuilder();

            foreach (var card in hand)
            {
                sb.Append(card.ToString());
                sb.Append(' ');
            }

            return sb.ToString();
        }
    }
}
