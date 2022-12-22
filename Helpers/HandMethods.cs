using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
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
                //if (group.Count() >= 5) continue;

                // First check if non-knocker cards can be added to knockers run melds. Non-knocker can potentially discard more than
                // 1 card on a sequence meld, whereas a maximum of 1 card can be discarded on a same-rank meld
                if (group.First().IsMeld3or4ofKind == false)
                {
                    var suit = group.First().Suit;

                    var handNonKnockerNonMelds_OfSameSuit = handNonKnockerNonMelds.Where(c => c.Suit == suit).ToList();

                    if (handNonKnockerNonMelds_OfSameSuit.Count > 0)
                    {
                        var combinationOfAllCards = handNonKnockerNonMelds_OfSameSuit;
                        combinationOfAllCards.AddRange(group);
                        combinationOfAllCards = SortHandBySuitThenRank(combinationOfAllCards);
                        List<Card> meld = new();

                        // TODO: see if refactoring with GroupMeldsAlgorithm method is possible
                        while (combinationOfAllCards.Count >= 3)
                        {
                            if (combinationOfAllCards[0].Rank != (combinationOfAllCards[1].Rank - 1) ||
                                combinationOfAllCards[0].Rank != (combinationOfAllCards[2].Rank - 2))
                            {
                                combinationOfAllCards.Remove(combinationOfAllCards.First());
                            }
                            else
                            {
                                meld = combinationOfAllCards.Take(3).ToList();
                                combinationOfAllCards.RemoveRange(0, 3);

                                while (combinationOfAllCards.Count() > 0)
                                {
                                    if (combinationOfAllCards.First().Rank == (meld.Last().Rank + 1))
                                    {
                                        meld.Add(combinationOfAllCards.First());
                                        combinationOfAllCards.Remove(combinationOfAllCards.First());
                                    }
                                    else break;
                                }
                            }
                        }
                        // TODO: (END....) see if refactoring with GroupMeldsAlgorithm method is possible

                        foreach (var card in meld)
                        {
                            if(handNonKnocker.Contains(card))
                            {
                                int findCardInHand = handNonKnocker.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
                                handNonKnocker[findCardInHand].IsInMeld = true;
                            }
                        }


                        //// ----------original-------------------
                        //var combinationOfAllCards = handNonKnockerNonMelds_OfSameSuit;
                        //combinationOfAllCards.AddRange(group);
                        //combinationOfAllCards = SortHand(combinationOfAllCards);

                        //List<int> ranks = new();

                        //foreach (var card in combinationOfAllCards)
                        //{
                        //    ranks.Add((int)card.Rank);
                        //}


                        ////while (ranks.Count > 3)
                        //{
                        //    if (ranks[ranks.Count - 1] != (ranks[ranks.Count - 2] + 1) ||
                        //    ranks[ranks.Count - 1] != (ranks[ranks.Count - 3] + 2))
                        //    {
                        //        ranks.Remove(ranks.Last());
                        //    }

                        //    if (ranks.Count < 3) break;

                        //    if (ranks[0] != (ranks[1] - 1) ||
                        //        ranks[0] != (ranks[2] - 2))
                        //    {
                        //        ranks.Remove(ranks.First());
                        //    }

                        //    if (ranks.Count < 3) break;

                        //    if (ranks.SequenceEqual(Enumerable.Range(ranks.First(), ranks.Count())))
                        //    {
                        //        foreach (var rank in ranks)
                        //        {
                        //            int findCardInHand = handNonKnocker.FindIndex(c => ((int)c.Rank == rank) && (c.Suit == suit));
                        //            if (findCardInHand >= 0) handNonKnocker[findCardInHand].IsInMeld = true;
                        //        }

                        //        break;
                        //    };
                        //}
                        //// ----------end original-------------------
                    }
                }

                else
                {
                    var rank = group.First().Rank;

                    var handNonKnockerNonMelds_OfSameRank = handNonKnockerNonMelds.Where(c => c.Rank == rank).ToList();

                    foreach (var card in handNonKnockerNonMelds_OfSameRank)
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

        public static List<Card> SortHandBySuitThenRank(List<Card> hand)
        {
            var sortedHand = hand.OrderBy(c => c.Suit).ThenBy(c => c.Rank).ToList();
            return sortedHand;
        }

        public static List<Card> SortHandWithMeldGroupings(List<Card> hand)
        {
            List<Card> sortedMelds = hand.Where(c => (c.IsInMeld == true)).OrderBy(c => c.MeldGroupIdentifier).ThenBy(c => c.Rank).ToList();
            List<Card> sortedNonMelds = hand.Where(c => (c.IsInMeld == false)).OrderBy(c => c.Rank).ToList();

            List<Card> sortedHand = sortedMelds;
            sortedHand.AddRange(sortedNonMelds);

            return sortedHand;
        }

        // TODO: meld sequences can be larger than size 5
        // TODO: write documentation explaining the algorithm
        public static List<Card> DetermineMeldsInHand(List<Card> hand)
        {
            var _hand = hand;

            foreach (var card in _hand)
            {
                card.IsInMeld = false;
                card.MeldGroupIdentifier = -1;
                card.IsMeld3or4ofKind = false;
            }

            #region Gather sequence melds
            // Find run combinations: for all sets of size greater than and equal to 3, run the Enumberable.Range algorithm to see if sequence exists
            List<List<Card>> largestSequenceMelds = new();
            List<List<Card>> cardsBySuit = new();
            //List<List<Card>> cardsBySuitOverflow = new List<List<Card>>();

            var handOrderedBySuitThenRank = SortHandBySuitThenRank(_hand);
            cardsBySuit.Add(handOrderedBySuitThenRank.Where(c => c.Suit == Suit.Spades).ToList());
            cardsBySuit.Add(handOrderedBySuitThenRank.Where(c => c.Suit == Suit.Clubs).ToList());
            cardsBySuit.Add(handOrderedBySuitThenRank.Where(c => c.Suit == Suit.Hearts).ToList());
            cardsBySuit.Add(handOrderedBySuitThenRank.Where(c => c.Suit == Suit.Diamonds).ToList());

            #region Old code to split groupings of 6 or higher into groups of maximum size 5. Delete this once everything else is finalized
            //// If there is potential of a meld of size 6 or greater, split them up into groups of maximum size 5
            //foreach (var suitGroup in cardsBySuit)
            //{
            //    if (suitGroup.Count() > 5)
            //    {
            //        List<Card> appendedSuitGroup = new();

            //        while (suitGroup.Count() > 5)
            //        {
            //            if (suitGroup[0].Rank == (suitGroup[1].Rank - 1) ||
            //                suitGroup[0].Rank == (suitGroup[2].Rank - 2))
            //            {
            //                if (suitGroup[0].Rank == (suitGroup[4].Rank - 4))
            //                {
            //                    // TODO: refactor into method
            //                    appendedSuitGroup = suitGroup.Take(5).ToList();
            //                    cardsBySuitOverflow.Add(appendedSuitGroup);
            //                    suitGroup.RemoveRange(0, 5);
            //                }

            //                else if (suitGroup[0].Rank == (suitGroup[3].Rank - 3))
            //                {
            //                    appendedSuitGroup = suitGroup.Take(4).ToList();
            //                    cardsBySuitOverflow.Add(appendedSuitGroup);
            //                    suitGroup.RemoveRange(0, 4);
            //                }

            //                else
            //                {
            //                    appendedSuitGroup = suitGroup.Take(3).ToList();
            //                    cardsBySuitOverflow.Add(appendedSuitGroup);
            //                    suitGroup.RemoveRange(0, 3);
            //                }
            //            }
            //            else
            //            {
            //                appendedSuitGroup = suitGroup.Take(1).ToList();
            //                cardsBySuitOverflow.Add(appendedSuitGroup);
            //                suitGroup.Remove(suitGroup.First());
            //            }
            //        }
            //    }
            //}

            //// Combine overflow list with original list
            //if (cardsBySuitOverflow.Count > 0)
            //{
            //    foreach (var list in cardsBySuitOverflow)
            //    {
            //        cardsBySuit.Add(list);
            //    }
            //}
            #endregion

            cardsBySuit = cardsBySuit.Where(list => list.Count > 0).ToList();

            // Gather groupings of the largest possible run-sequence melds
            foreach (var cards in cardsBySuit)
            {
                if (cards.Count < 3) continue;

                List<Card> meld = new();

                while (cards.Count >= 3)
                {
                    if (cards[0].Rank != (cards[1].Rank - 1) ||
                        cards[0].Rank != (cards[2].Rank - 2))
                    {
                        cards.Remove(cards.First());
                    }
                    else
                    {
                        meld = cards.Take(3).ToList();
                        cards.RemoveRange(0, 3);

                        while (cards.Count() > 0)
                        {
                            if (cards.First().Rank == (meld.Last().Rank + 1))
                            {
                                meld.Add(cards.First());
                                cards.Remove(cards.First());
                            }
                            else break;
                        }

                        largestSequenceMelds.Add(meld);
                    }
                }
            }
            #endregion

            #region Gather 3 or 4 of kind melds

            List<List<Card>> largestSameRankMelds = new();

            var handGroupedByRank = _hand.GroupBy(c => c.Rank).ToList();

            foreach (var rank in handGroupedByRank)
            {
                if (rank.Count() < 3) continue;

                largestSameRankMelds.Add(rank.ToList());
            }
            #endregion

            List<List<Card>> allPossibleMelds = new();
            #region Determine all possible sequence melds

            #region Working method but it creates duplicates
            //// Sequence melds
            //foreach (var meld in largestSequenceMelds)
            //{
            //    allPossibleMelds.Add(meld);

            //    if (meld.Count == 3) continue;

            //    List<Card> temp = new();
            //    foreach(var card in meld)
            //    {
            //        temp.Add(card);
            //    }

            //    while (temp.Count > 3)
            //    {
            //        var removedFirstElement = temp.Skip(1).ToList();
            //        var removedLastElement = temp.Take(temp.Count - 1).ToList();

            //        allPossibleMelds.Add(removedFirstElement);
            //        allPossibleMelds.Add(removedLastElement);

            //        temp.Remove(temp.Last());
            //    }

            //    temp.Clear();
            //    foreach (var card in meld)
            //    {
            //        temp.Add(card);
            //    }

            //    while (temp.Count > 3)
            //    {
            //        var removedFirstElement = temp.Skip(1).ToList();
            //        var removedLastElement = temp.Take(temp.Count - 1).ToList();

            //        allPossibleMelds.Add(removedFirstElement);
            //        allPossibleMelds.Add(removedLastElement);

            //        temp.Remove(temp.First());
            //    }
            #endregion

            // Add every possible sequence meld to allPossibleMelds
            foreach (var sequence in largestSequenceMelds)
            {
                allPossibleMelds.Add(sequence);

                if (sequence.Count == 3) continue;

                int maxIndexPosition = sequence.Count - 3;
                int maxMeldSize = sequence.Count - 1;

                List<Card> subMeld = new();

                for (int meldSize = 3; meldSize <= maxMeldSize; meldSize++)
                {
                    for (int index = 0; index <= maxIndexPosition; index++)
                    {
                        subMeld = sequence.Skip(index).Take(meldSize).ToList();

                        allPossibleMelds.Add(subMeld);
                    }

                    maxIndexPosition--;
                }
            }

            #endregion

            #region Determine all possible 3/4 of kind melds

            foreach (var sameRankList in largestSameRankMelds)
            {
                allPossibleMelds.Add(sameRankList);

                if (sameRankList.Count == 3) continue;

                // If this line is reached in code, then "sameRankList" is a 4-of-a-kind
                // Add all 3-of-a-kind combinations to "allPossibleMelds"

                List<Card> copy = new();
                foreach (var card in sameRankList)
                {
                    copy.Add(card);
                }

                for (int index = 0; index < sameRankList.Count; index++)
                {
                    List<Card> sameRankList_OneCardRemoved = new();
                    sameRankList_OneCardRemoved.AddRange(copy);
                    sameRankList_OneCardRemoved.RemoveAt(index);
                    allPossibleMelds.Add(sameRankList_OneCardRemoved);
                }
            }

            #endregion

            if (allPossibleMelds.Count == 0)
            {
                return SortHandWithMeldGroupings(_hand);
            }

            #region Determine all combinations of the possible melds

            List<List<List<Card>>> meldCombinationsWithoutDuplicateCards = new();

            // TODO: test with hand where only 1 meld exists
            //foreach(var meld in allPossibleMelds)
            for (int numMelds = 0; numMelds < allPossibleMelds.Count; numMelds++)
            {
                // Need to find all other melds that contain the cards in "meld", so that they can be eliminated from potential combinations list
                var meld = allPossibleMelds[numMelds];

                List<int> indexOfMeldsThatContainSameCards = new();

                for (int index = 0; index < allPossibleMelds.Count; index++)
                {
                    var list = allPossibleMelds[index];

                    foreach (var card in list)
                    {
                        if (meld.Contains(card))
                        {
                            indexOfMeldsThatContainSameCards.Add(index);
                            break;
                        }
                    }
                }

                List<List<Card>> meldsOfDifferentCards = new();

                for (int index = 0; index < allPossibleMelds.Count; index++)
                {
                    if (indexOfMeldsThatContainSameCards.Contains(index)) continue;

                    meldsOfDifferentCards.Add(allPossibleMelds[index]);
                }


                if (meldsOfDifferentCards.Count == 0)
                {
                    List<List<Card>> uniqueMeldCombination = new();
                    uniqueMeldCombination.Add(meld);
                    meldCombinationsWithoutDuplicateCards.Add(uniqueMeldCombination);
                    continue;
                }

                for (int index = 0; index < meldsOfDifferentCards.Count; index++)
                {
                    var guaranteedMeldPair = meldsOfDifferentCards[index];

                    List<List<Card>> uniqueMeldCombination = new();
                    uniqueMeldCombination.Add(meld);
                    uniqueMeldCombination.Add(guaranteedMeldPair);

                    List<List<Card>> additionalMelds = new();
                    foreach (var list in meldsOfDifferentCards)
                    {
                        additionalMelds.Add(list);
                    }

                    additionalMelds.RemoveAt(index);

                    List<int> indexOfPotentialMeldsThatContainSameCards = new();

                    for (int index_ = 0; index_ < additionalMelds.Count; index_++)
                    {
                        var potentialAdditionalMeld = additionalMelds[index_];

                        foreach (var card in potentialAdditionalMeld)
                        {
                            if (guaranteedMeldPair.Contains(card))
                            {
                                indexOfPotentialMeldsThatContainSameCards.Add(index_);
                                break;
                            }
                        }
                    }

                    for (int index_ = 0; index_ < additionalMelds.Count; index_++)
                    {
                        if (indexOfPotentialMeldsThatContainSameCards.Contains(index_)) continue;

                        uniqueMeldCombination.Add(additionalMelds[index_]);
                    }

                    if (!meldCombinationsWithoutDuplicateCards.Contains(uniqueMeldCombination))
                    {
                        meldCombinationsWithoutDuplicateCards.Add(uniqueMeldCombination);
                    }
                }
            }

            //// Group "meldCombinationsWithoutDuplicateCards" into sets containing the same number of melds. If there is more than one group,
            //// remove groups that do not contain the highest number of melds (since the lowest possible hand value will always correspond
            //// to a hand with the highest number of melds)

            //var meldCombinationsWithoutDuplicateCards_Groups = meldCombinationsWithoutDuplicateCards.GroupBy(melds => melds.Count);

            //int largestGroupSize = 0;
            //foreach(var group in meldCombinationsWithoutDuplicateCards_Groups)
            //{
            //    if (group.Key <= largestGroupSize) continue;

            //    largestGroupSize = group.Key;
            //}

            //meldCombinationsWithoutDuplicateCards = meldCombinationsWithoutDuplicateCards.Where(numOfMelds => (numOfMelds.Count == largestGroupSize)).ToList();

            #endregion

            #region Determine non-melded cards and add them to list to create the complete melded & non-melded hand

            int handValue;
            int lowestHandValue = int.MaxValue;
            List<List<Card>> bestMeldCombination = new();
            List<Card> nonMeldedCards = new();

            foreach (var meldCombination in meldCombinationsWithoutDuplicateCards)
            {
                var meldedCards = meldCombination.SelectMany(c => c).ToList();
                var _nonMeldedCards = hand.Except(meldedCards).ToList();

                handValue = CalculateHandValue(_nonMeldedCards);

                if (handValue >= lowestHandValue) continue;

                lowestHandValue = handValue;
                bestMeldCombination = meldCombination;
                nonMeldedCards = _nonMeldedCards;
            }

            // Set Card properties for the best hand
            int meldGroupNum = 0;
            foreach(var meld in bestMeldCombination)
            {
                var suit = meld[0].Suit;
                var isMeldARun = meld.All(card => (card.Suit == suit));

                if(!isMeldARun)
                {
                    foreach(var card in meld)
                    {
                        card.IsMeld3or4ofKind = true;
                    }
                }

                foreach (var card in meld)
                {
                    card.IsInMeld = true;
                    card.MeldGroupIdentifier = meldGroupNum;
                }

                meldGroupNum++;
            }

            List<Card> bestHand = new();
            bestHand.AddRange(bestMeldCombination.SelectMany(c => c).ToList());
            bestHand.AddRange(nonMeldedCards);

            #endregion

            return SortHandWithMeldGroupings(bestHand);
        }

        // TODO: delete all commented out stuff
        //public static List<Card> DetectAndGroupByMelds(List<Card> hand)
        //{
        //    foreach (var card in hand)
        //    {
        //        // Reset all meld properties in case player decided to break meld during discard
        //        card.IsInMeld = false;
        //        card.MeldGroupIdentifier = -1;
        //        card.IsMeld3or4ofKind = false;
        //    }

        //    hand = SortHand(hand);

        //    #region Check for runs of three or more
        //    List<List<Card>> cardsBySuit = new List<List<Card>>();

        //    cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Spades).ToList());
        //    cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Clubs).ToList());
        //    cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Hearts).ToList());
        //    cardsBySuit.Add(hand.Where(c => c.Suit == Suit.Diamonds).ToList());

        //    int meldGroupIdentifier = 0;

        //    foreach (var cards in cardsBySuit)
        //    {
        //        // TODO: bug where doesn't count meld of 234678 same suit
        //        // TODO: refactor to use "continue" statement if condition isn't met, instead of nesting statements
        //        if (cards.Count() >= 3)
        //        {
        //            List<int> ranks = new();

        //            foreach (var card in cards)
        //            {
        //                ranks.Add((int)card.Rank);
        //            }

        //            if (ranks.Count > 3)
        //            {
        //                while (ranks.Count > 3)
        //                {
        //                    if (ranks[ranks.Count - 1] != (ranks[ranks.Count - 2] + 1) ||
        //                    ranks[ranks.Count - 1] != (ranks[ranks.Count - 3] + 2))
        //                    {
        //                        ranks.Remove(ranks.Last());
        //                        cards.Remove(cards.Last());
        //                    }

        //                    if (ranks.Count < 3) break;

        //                    if (ranks[0] != (ranks[1] - 1) ||
        //                        ranks[0] != (ranks[2] - 2))
        //                    {
        //                        ranks.Remove(ranks.First());
        //                        cards.Remove(cards.First());
        //                    }

        //                    if (ranks.Count < 3) break;

        //                    if (ranks.SequenceEqual(Enumerable.Range(ranks.First(), ranks.Count())))
        //                    {
        //                        foreach (var card in cards)
        //                        {
        //                            int findCardInHand = hand.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
        //                            hand[findCardInHand].IsInMeld = true;
        //                            hand[findCardInHand].MeldGroupIdentifier = meldGroupIdentifier;
        //                            hand[findCardInHand].IsMeld3or4ofKind = false;
        //                        }

        //                        meldGroupIdentifier++;

        //                        break;
        //                    };
        //                }
        //            }

        //            else
        //            {
        //                if (ranks.SequenceEqual(Enumerable.Range(ranks.First(), ranks.Count())))
        //                {
        //                    foreach (var card in cards)
        //                    {
        //                        int findCardInHand = hand.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
        //                        hand[findCardInHand].IsInMeld = true;
        //                        hand[findCardInHand].MeldGroupIdentifier = meldGroupIdentifier;
        //                        hand[findCardInHand].IsMeld3or4ofKind = false;
        //                    }

        //                    meldGroupIdentifier++;
        //                };
        //            }
        //        }
        //    }
        //    #endregion

        //    #region Check for three/four of a kind
        //    var handNotInMeld = hand.Where(c => (c.IsInMeld == false));
        //    var groupByRank = handNotInMeld.GroupBy(c => c.Rank);

        //    foreach (var group in groupByRank)
        //    {
        //        // TODO: refactor to use continue statement
        //        if (group.Count() >= 3)
        //        {
        //            foreach (var card in group)
        //            {
        //                int findCardInHand = hand.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
        //                hand[findCardInHand].IsInMeld = true;
        //                hand[findCardInHand].MeldGroupIdentifier = meldGroupIdentifier;
        //                hand[findCardInHand].IsMeld3or4ofKind = true;
        //            }

        //            meldGroupIdentifier++;
        //        }
        //    }
        //    #endregion

        //    return SortHandWithMeldGroupings(hand);
        //}

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
