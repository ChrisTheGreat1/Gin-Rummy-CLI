using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace _11242022_Gin_Rummy.Helpers
{
    public static class HandMethods
    {
        public static List<Card> NonKnockerCombinesUnmatchedCardsWithKnockersMelds(List<Card> handKnocker, List<Card> handNonKnocker)
        {
            List<Card> handOfMeldAndPotentialDiscards = new();

            var handKnockerMelds = handKnocker.Where(c => c.IsInMeld).ToList();
            var handNonKnockerNonMelds = handNonKnocker.Where(c => !c.IsInMeld).ToList();

            var groupsOfMelds = handKnockerMelds.GroupBy(c => c.MeldGroupIdentifier);

            foreach (var group in groupsOfMelds)
            {
                if (group.Count() >= 5) continue;

                if (group.First().IsMeld3or4ofKind == false)
                {
                    var suit = group.First().Suit;

                    var handNonKnockerNonMelds_OfSameSuit = handNonKnockerNonMelds.Where(c => c.Suit == suit).ToList();

                    if (handNonKnockerNonMelds_OfSameSuit.Count > 0)
                    {
                        var combinationOfAllCards = handNonKnockerNonMelds_OfSameSuit;
                        combinationOfAllCards.AddRange(group);
                        combinationOfAllCards = SortHand(combinationOfAllCards);

                        List<int> ranks = new();

                        foreach (var card in combinationOfAllCards)
                        {
                            ranks.Add((int)card.Rank);
                        }

                        // TODO: see if refactoring with GroupMelds method is possible
                        while (ranks.Count > 3)
                        {
                            if (ranks[ranks.Count - 1] != (ranks[ranks.Count - 2] + 1) ||
                            ranks[ranks.Count - 1] != (ranks[ranks.Count - 3] + 2))
                            {
                                ranks.Remove(ranks.Last());
                            }

                            if (ranks.Count < 3) break;

                            if (ranks[0] != (ranks[1] - 1) ||
                                ranks[0] != (ranks[2] - 2))
                            {
                                ranks.Remove(ranks.First());
                            }

                            if (ranks.Count < 3) break;

                            if (ranks.SequenceEqual(Enumerable.Range(ranks.First(), ranks.Count())))
                            {
                                foreach (var rank in ranks)
                                {
                                    int findCardInHand = handNonKnocker.FindIndex(c => ((int)c.Rank == rank) && (c.Suit == suit));
                                    if (findCardInHand >= 0) handNonKnocker[findCardInHand].IsInMeld = true;
                                }

                                break;
                            };
                        }
                    }
                }

                else
                {
                    var rank = group.First().Rank;

                    var handNonKnockerNonMelds_OfSameRank = handNonKnockerNonMelds.Where(c => c.Rank == rank).ToList();

                    foreach(var card in handNonKnockerNonMelds_OfSameRank)
                    {
                        int findCardInHand = handNonKnocker.FindIndex(c => (c.Rank == rank) && (c.Suit == card.Suit));
                        handNonKnocker[findCardInHand].IsInMeld = true;
                    }
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

            foreach (var card in hand)
            {
                // TODO: refactor to use continue statement
                if (!card.IsInMeld)
                {
                    if (card.Rank == Rank.Jack || card.Rank == Rank.Queen || card.Rank == Rank.King) handValue += 10;
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

        // TODO: write documentation explaining the algorithm
        // TODO: need to prioritize making groups of 3 first. See edge case: 4s 5s 6s 7s 6c 7c 8c 6h 7h 7d. Maybe need algo that tries all combinations then SELECTS THE ONE WITH LOWEST HAND VALUE
        // 4s 5s 6s 7s 6c 7c 8c 6h 7h 7d
        // Ac 2c 3c Qs Qc Qh 2s 2d 3s 3h
        public static List<Card> DetectAndGroupByMelds(List<Card> hand)
        {
            foreach (var card in hand)
            {
                // Reset all meld properties in case player decided to break meld during discard
                card.IsInMeld = false;
                card.MeldGroupIdentifier = -1;
                card.IsMeld3or4ofKind = false;
            }

            hand = SortHand(hand);

            #region Check for runs of three or more
            List<List<Card>> cardsBySuit = new List<List<Card>>();

            cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Spades).ToList());
            cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Clubs).ToList());
            cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Hearts).ToList());
            cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Diamonds).ToList());

            int meldGroupIdentifier = 0;

            foreach (var cards in cardsBySuit)
            {
                // TODO: refactor to use "continue" statement if condition isn't met, instead of nesting statements
                if (cards.Count() >= 3)
                {
                    List<int> ranks = new();

                    foreach (var card in cards)
                    {
                        ranks.Add((int)card.Rank);
                    }

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
                                foreach (var card in cards)
                                {
                                    int findCardInHand = hand.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
                                    hand[findCardInHand].IsInMeld = true;
                                    hand[findCardInHand].MeldGroupIdentifier = meldGroupIdentifier;
                                    hand[findCardInHand].IsMeld3or4ofKind = false;
                                }

                                meldGroupIdentifier++;

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
                                hand[findCardInHand].MeldGroupIdentifier = meldGroupIdentifier;
                                hand[findCardInHand].IsMeld3or4ofKind = false;
                            }

                            meldGroupIdentifier++;
                        };
                    }
                }
            }
            #endregion

            #region Check for three/four of a kind
            var handNotInMeld = hand.Where(c => (c.IsInMeld == false));
            var groupByRank = handNotInMeld.GroupBy(c => c.Rank);

            foreach (var group in groupByRank)
            {
                // TODO: refactor to use continue statement
                if (group.Count() >= 3)
                {
                    foreach (var card in group)
                    {
                        int findCardInHand = hand.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
                        hand[findCardInHand].IsInMeld = true;
                        hand[findCardInHand].MeldGroupIdentifier = meldGroupIdentifier;
                        hand[findCardInHand].IsMeld3or4ofKind = true;
                    }

                    meldGroupIdentifier++;
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
