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
        /// <summary>
        /// Method to combine the non-knockers non-melded cards with the knockers melds. 
        /// </summary>
        /// <param name="handKnocker">Hand of the player that knocked.</param>
        /// <param name="handNonKnocker">Hand of the player that did not knock.</param>
        /// <returns>Non-knockers hand with card properties set to show they are part of a meld, if they could be combined with the knockers melds.</returns>
        public static List<Card> NonKnockerCombinesUnmatchedCardsWithKnockersMelds(List<Card> handKnocker, List<Card> handNonKnocker)
        {
            List<Card> handNonKnockerAfterDiscardingOntoOpponentMelds = handNonKnocker;
            List<Card> handOfMeldAndPotentialDiscards = new();

            var handKnockerMelds = handKnocker.Where(c => c.IsInMeld).ToList();
            var handNonKnockerNonMelds = handNonKnocker.Where(c => !c.IsInMeld).ToList();

            var groupsOfMelds = handKnockerMelds.GroupBy(c => c.MeldGroupIdentifier);

            foreach (var group in groupsOfMelds)
            {
                // First check if non-knocker cards can be added to knocker's run melds. Non-knocker can potentially discard more than
                // 1 card on opponents sequence meld, whereas a maximum of 1 card can be discarded on opponents three-of-a-kind meld.
                if (group.First().IsMeld3or4ofKind == false)
                {
                    var suit = group.First().Suit;

                    var handNonKnockerNonMelds_OfSameSuit = handNonKnockerNonMelds.Where(c => c.Suit == suit).ToList();

                    if (handNonKnockerNonMelds_OfSameSuit.Count > 0)
                    {
                        // Combine all cards, then rerun the ExtractLongestSequence algorithm to see if any non-knocker non-meld cards can be introduced
                        var combinationOfAllCards = handNonKnockerNonMelds_OfSameSuit;
                        combinationOfAllCards.AddRange(group);
                        combinationOfAllCards = SortHandBySuitThenRank(combinationOfAllCards);

                        var melds = ExtractLongestSequence(combinationOfAllCards);

                        foreach(var meld in melds)
                        {
                            // If the meld contains any non-knocker non-meld cards, update the card properties to denote the card as belonging to a meld.
                            // This will cause the discard to not be counted when calculating the non-knocker's hand value
                            foreach (var card in meld)
                            {
                                if (handNonKnocker.Contains(card))
                                {
                                    int findCardInHand = handNonKnockerAfterDiscardingOntoOpponentMelds.FindIndex(c => (c.Rank == card.Rank) && (c.Suit == card.Suit));
                                    handNonKnockerAfterDiscardingOntoOpponentMelds[findCardInHand].IsInMeld = true;
                                }
                            }
                        }
                    }
                }

                // Check if non-knocker cards can be added to knocker's same-rank melds
                else
                {
                    var rank = group.First().Rank;

                    var handNonKnockerNonMelds_OfSameRank = handNonKnockerNonMelds.Where(c => c.Rank == rank).ToList();

                    // If the meld contains any non-knocker non-meld cards, update the card properties to denote the card as belonging to a meld.
                    // This will cause the discard to not be counted when calculating the non-knocker's hand value
                    foreach (var card in handNonKnockerNonMelds_OfSameRank)
                    {
                        int findCardInHand = handNonKnockerAfterDiscardingOntoOpponentMelds.FindIndex(c => (c.Rank == rank) && (c.Suit == card.Suit));
                        handNonKnockerAfterDiscardingOntoOpponentMelds[findCardInHand].IsInMeld = true;
                    }
                }
            }

            return handNonKnockerAfterDiscardingOntoOpponentMelds;
        }

        /// <summary>
        /// Returns an integer denoting the value of the player's hand. 
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static int CalculateHandValue(List<Card> hand)
        {
            int handValue = 0;

            foreach (var card in hand)
            {
                if (card.IsInMeld) continue;

                if (card.Rank == Rank.Jack || card.Rank == Rank.Queen || card.Rank == Rank.King) handValue += 10;
                else handValue += (int)card.Rank;

            }

            return handValue;
        }

        /// <summary>
        /// Returns a boolean denoting if the player can knock (ie. the player's hand value is equal to 10 or less).
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static bool CanPlayerKnock(List<Card> hand)
        {
            if (CalculateHandValue(hand) <= 10) return true;
            else return false;
        }

        // Returns a boolean denoting if the player's hand meets gin criteria (ie. all cards are part of a meld).
        public static bool DetectGin(List<Card> hand)
        {
            if (hand.All(c => c.IsInMeld)) return true;
            else return false;
        }

        /// <summary>
        /// Returns the passed-in list sorted by suit then by rank in ascending order.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static List<Card> SortHandBySuitThenRank(List<Card> hand)
        {
            return hand.OrderBy(c => c.Suit).ThenBy(c => c.Rank).ToList();
        }

        /// <summary>
        /// Returns the passed-in list sorted by melds and non-melds.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static List<Card> SortHandWithMeldGroupings(List<Card> hand)
        {
            List<Card> sortedMelds = hand.Where(c => (c.IsInMeld == true)).OrderBy(c => c.MeldGroupIdentifier).ThenBy(c => c.Rank).ToList();
            List<Card> sortedNonMelds = hand.Where(c => (c.IsInMeld == false)).OrderBy(c => c.Rank).ToList();

            List<Card> sortedHand = sortedMelds;
            sortedHand.AddRange(sortedNonMelds);

            return sortedHand;
        }

        /// <summary>
        /// Returns the passed-in list with the optimal meld groupings that provide the lowest hand score.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static List<Card> DetermineMeldsInHand(List<Card> hand)
        {
            // Create copy of hand so the original input argument is not being modified
            var _hand = hand;

            // Reset every card property in the hand to defaults in case the player broke the meld during their most recent turn
            foreach (var card in _hand)
            {
                card.IsInMeld = false;
                card.MeldGroupIdentifier = -1;
                card.IsMeld3or4ofKind = false;
            }

            // Determine all possible sequence/run melds
            var handSortedBySuit = SortHandBySuit(_hand);
            handSortedBySuit = handSortedBySuit.Where(list => list.Count >= 3).ToList(); // Filter out any groups with less than 3 cards as meld can't form
            List<List<Card>> longestSequenceMelds = FindLongestSequenceMelds(handSortedBySuit);
            var allPossibleSequenceMelds = DetermineAllPossibleSequenceMelds(longestSequenceMelds);

            // Obtain all 3/4 of a kind melds
            var sameRankMelds = FindLargestSameRankMelds(_hand);

            // If 4 of a kind exists, also determine all possible 3 of a kind sub melds that can be made
            var allPossibleSameRankMelds = DetermineAllPossibleSameRankMelds(sameRankMelds);

            // Gather all possible sequence and rank melds into a single list
            List<List<Card>> allPossibleMelds = new();

            foreach (var sequenceMeld in allPossibleSequenceMelds)
            {
                allPossibleMelds.Add(sequenceMeld);
            }

            foreach (var rankMeld in allPossibleSameRankMelds)
            {
                allPossibleMelds.Add(rankMeld);
            }

            // If no melds exist, simply sort the original hand and return it
            if (allPossibleMelds.Count == 0)
            {
                return SortHandWithMeldGroupings(_hand);
            }

            // Gather all possible combinations of melds (with no duplicated cards) into a list
            var meldCombinations = DetermineAllCombinationsOfMelds(allPossibleMelds);

            // Maximum number of melds possible in hand is 3
            // Filter out higher meld counts to prevent bugs, particularly during SimpleAgent play when 11 cards are being evaluated in a hand
            meldCombinations = meldCombinations.Where(m => m.Count < 4).ToList(); 

            // Evaluate the point values for all possible meld combinations and return the best possible hand
            var bestHand = DetermineBestPossibleHand(_hand, meldCombinations);

            return SortHandWithMeldGroupings(bestHand);
        }

        /// <summary>
        /// Determine the meld groupings that provide the lowest hand value.
        /// </summary>
        /// <param name="hand">Complete player hand of 10 cards.</param>
        /// <param name="meldCombinations">List of all possible meld combinations from the player hand.</param>
        /// <returns>List<Card> with Card properties set for the optimal meld groupings.</returns>
        private static List<Card> DetermineBestPossibleHand(List<Card> hand, List<List<List<Card>>> meldCombinations)
        {
            List<List<Card>> bestMeldCombination = new();
            List<Card> nonMeldedCards = new();

            int handValue;
            int lowestHandValue = int.MaxValue;

            // Loop through each meld combination and determine which cards are not present.
            // Calculate the hand value of the non-present cards (ie. the cards which would not be in a meld).
            // Set the hand with the lowest value as the "bestMeldCombination"
            foreach (var meldCombination in meldCombinations)
            {
                var meldedCards = meldCombination.SelectMany(c => c).ToList();
                var _nonMeldedCards = hand.Except(meldedCards).ToList();

                handValue = CalculateHandValue(_nonMeldedCards);

                if (handValue >= lowestHandValue) continue;

                lowestHandValue = handValue;
                bestMeldCombination = meldCombination;
                nonMeldedCards = _nonMeldedCards;
            }

            bestMeldCombination = AssignMeldPropertiesToBestCombination(bestMeldCombination);

            List<Card> bestHand = new();
            bestHand.AddRange(bestMeldCombination.SelectMany(c => c).ToList());
            bestHand.AddRange(nonMeldedCards);

            return bestHand;
        }

        /// <summary>
        /// Assign Card properties to all of the players cards that belong to the optimal meld groupings.
        /// </summary>
        /// <param name="bestMeldCombination"></param>
        /// <returns></returns>
        private static List<List<Card>> AssignMeldPropertiesToBestCombination(List<List<Card>> bestMeldCombination)
        {
            var _bestMeldCombination = bestMeldCombination;

            int meldGroupNum = 0;
            foreach (var meld in _bestMeldCombination)
            {
                var suit = meld.First().Suit;
                var isMeldARun = meld.All(card => (card.Suit == suit));

                if (!isMeldARun)
                {
                    foreach (var card in meld)
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

            return _bestMeldCombination;
        }

        /// <summary>
        /// Determine all possible combinations of melds from the players hand. Combinations never include duplicate/repeated cards.
        /// </summary>
        /// <param name="allPossibleMelds"></param>
        /// <returns></returns>
        private static List<List<List<Card>>> DetermineAllCombinationsOfMelds(List<List<Card>> allPossibleMelds)
        {
            List<List<List<Card>>> meldCombinations = new();

            // Loop through each meld. Find all other melds that can be combined with it (ie. find all other melds that do not contain duplicated cards)
            for (int numMelds = 0; numMelds < allPossibleMelds.Count; numMelds++)
            {
                // Need to find all other melds that contain the cards in "meld", so that they can be eliminated from potential combinations list.
                // Otherwise meld combinations with duplicate cards will be created, ex. 4567 of spades meld will be combined with 456 of spades meld
                var meld = allPossibleMelds[numMelds];

                var meldsNotContainingDuplicateCards = FindMeldsThatDoNotContainDuplicateCards(allPossibleMelds, meld);

                // If no other meld pairs can be combined with the original meld, simply add the original meld to the list then move on to next loop
                if (meldsNotContainingDuplicateCards.Count == 0)
                {
                    List<List<Card>> uniqueMeldCombination = new();
                    uniqueMeldCombination.Add(meld);
                    meldCombinations.Add(uniqueMeldCombination);
                    continue;
                }

                // Repeat the above algorithm to find all meld pairings that do not contain duplicate cards.
                // Ex. "meld" = 456 of spades, and "meldsNotContainingDuplicateCards" contains 789 of diamonds and 9s9d9h 3-of-a-kind
                // Ex. The only possible meld combinations are 456 of spades + 789 of diamonds, or 456 of spades + 9s9d9h, since the 9d cannot be duplicated
                // Loop through every meld pairing. Find all other meld pairings that can be combined with it and add the complete combination to "meldCombinations"
                for (int index = 0; index < meldsNotContainingDuplicateCards.Count; index++)
                {
                    var guaranteedMeldPair = meldsNotContainingDuplicateCards[index];

                    var uniqueMeldCombination = FindMeldsThatDoNotContainDuplicateCards(meldsNotContainingDuplicateCards, guaranteedMeldPair);

                    uniqueMeldCombination.Add(meld);
                    uniqueMeldCombination.Add(guaranteedMeldPair);

                    if (!meldCombinations.Contains(uniqueMeldCombination))
                    {
                        meldCombinations.Add(uniqueMeldCombination);
                    }
                }
            }

            return meldCombinations;
        }

        /// <summary>
        /// Given a particular meld, find all of the other meld combinations that do not contain cards that are already in the particular meld.
        /// Return the list of meld combinations that contain only non-repeated cards.
        /// </summary>
        /// <param name="listOfMelds">List of meld combinations to be analyzed for repeated cards from the particular meld.</param>
        /// <param name="meld">The particular meld.</param>
        /// <returns>Meld combinations that do not include cards that are already found in the "meld" argument.</returns>
        private static List<List<Card>> FindMeldsThatDoNotContainDuplicateCards(List<List<Card>> listOfMelds, List<Card> meld)
        {
            List<int> listIndexesThatContainDuplicateCards = new();
            List<List<Card>> meldsNotContainingDuplicateCards = new();

            for (int index = 0; index < listOfMelds.Count; index++)
            {
                var list = listOfMelds[index];

                foreach (var card in list)
                {
                    if (meld.Contains(card))
                    {
                        // If the potential meld pair contains a card that is already in the original "meld" being analyzed, add
                        // the index position of the potential meld pair to a list so that the index position is excluded in the
                        // next step of the algorithm
                        listIndexesThatContainDuplicateCards.Add(index);
                        break;
                    }
                }
            }

            // Add every meld pair whose index is NOT found in "listIndexesThatContainDuplicateCards" to the list
            for (int index = 0; index < listOfMelds.Count; index++)
            {
                if (listIndexesThatContainDuplicateCards.Contains(index)) continue;

                meldsNotContainingDuplicateCards.Add(listOfMelds[index]);
            }

            return meldsNotContainingDuplicateCards;
        }

        /// <summary>
        /// Return a list of all the possible same rank melds. If a 4-of-a-kind meld is passed in, this method returns a list of the original argument as well
        /// as all of the possible 3-of-a-kind submelds that can be created. If a 3-of-a-kind is passed in, it is immediately returned.
        /// </summary>
        /// <param name="sameRankMelds">List of all melds that are a 3 or 4-of-a-kind.</param>
        /// <returns></returns>
        private static List<List<Card>> DetermineAllPossibleSameRankMelds(List<List<Card>> sameRankMelds)
        {
            List<List<Card>> allPossibleSameRankMelds = new();

            foreach (var sameRankList in sameRankMelds)
            {
                allPossibleSameRankMelds.Add(sameRankList); // Add the original meld to the list immediately in case it is only 3-of-a-kind

                if (sameRankList.Count == 3) continue; // Only run the following algorithm if the original sequence is a 4-of-a-kind

                // The following code obtains all possible 3-of-a-kind submelds that can be made from the original 4-of-a-kind

                // Create a copy of sameRankList so the original is not modified during following algorithm
                List<Card> _sameRankList = new();
                foreach (var card in sameRankList)
                {
                    _sameRankList.Add(card);
                }

                // Loop through each card in the 4-of-a-kind, delete it, then add the remaining 3-of-a-kind to the list
                for (int index = 0; index < sameRankList.Count; index++)
                {
                    List<Card> sameRankList_OneCardRemoved = new();
                    sameRankList_OneCardRemoved.AddRange(_sameRankList);
                    sameRankList_OneCardRemoved.RemoveAt(index);
                    allPossibleSameRankMelds.Add(sameRankList_OneCardRemoved);
                }
            }

            return allPossibleSameRankMelds;
        }

        /// <summary>
        /// Return a list of all the possible sequence melds. For example, if a sequence meld of length 4 is passed in, this method returns a list of the original argument 
        /// as well as all of the possible length-3 sequence submelds that can be created. If a length 3 sequence meld is passed in, it is immediately returned.
        /// </summary>
        /// <param name="longestSequenceMelds">List of all unmodified (ie. the longest possible length) sequence melds.</param>
        /// <returns></returns>
        private static List<List<Card>> DetermineAllPossibleSequenceMelds(List<List<Card>> longestSequenceMelds)
        {
            List<List<Card>> allPossibleSequenceMelds = new();

            foreach (var sequence in longestSequenceMelds)
            {
                allPossibleSequenceMelds.Add(sequence); // Add the original sequence to the list immediately in case it is only of length 3

                if (sequence.Count == 3) continue; // Only run the following algorithm if the original sequence is greater than length 3

                int maxIndexPosition = sequence.Count - 3; // Ensure the algorithm does not attempt card indexes that are out of bounds
                int maxMeldSize = sequence.Count - 1;

                List<Card> subMeld = new();

                // Start with "meldSize" of 3 and obtain all melds of length 3 possible. Then increase until "meldSize" is 1 less than original full meld length
                // Ex. "sequence" is 2345 of spades. Add the two submelds of 234 and 345 to "allPossibleSequenceMelds"
                for (int meldSize = 3; meldSize <= maxMeldSize; meldSize++)
                {
                    for (int index = 0; index <= maxIndexPosition; index++)
                    {
                        subMeld = sequence.Skip(index).Take(meldSize).ToList();

                        allPossibleSequenceMelds.Add(subMeld);
                    }

                    maxIndexPosition--;
                }
            }

            return allPossibleSequenceMelds;
        }

        /// <summary>
        /// Return the list of all 3 or (unmodified) 4-of-a-kind melds.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private static List<List<Card>> FindLargestSameRankMelds(List<Card> hand)
        {
            List<List<Card>> sameRankMelds = new();

            var _hand = hand.GroupBy(c => c.Rank).ToList();

            // Loop through each rank grouping. If less than 3 cards are present in the group (ie. it is not a 3/4 of kind), skip over it.
            // Otherwise add the 3/4 of a kind meld to the list
            foreach (var rank in _hand)
            {
                if (rank.Count() < 3) continue;

                sameRankMelds.Add(rank.ToList());
            }

            return sameRankMelds;
        }

        /// <summary>
        /// Return a list of the players hand sorted into sub-lists according to suit.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        private static List<List<Card>> SortHandBySuit(List<Card> hand)
        {
            List<List<Card>> handSortedBySuit = new();

            var _hand = SortHandBySuitThenRank(hand);
            handSortedBySuit.Add(_hand.Where(c => c.Suit == Suit.Spades).ToList());
            handSortedBySuit.Add(_hand.Where(c => c.Suit == Suit.Clubs).ToList());
            handSortedBySuit.Add(_hand.Where(c => c.Suit == Suit.Hearts).ToList());
            handSortedBySuit.Add(_hand.Where(c => c.Suit == Suit.Diamonds).ToList());

            return handSortedBySuit;
        }

        /// <summary>
        /// Return a list of the longest possible sequence melds that can be obtained from the player's hand after it has been sorted by suit.
        /// </summary>
        /// <param name="handSortedBySuit"></param>
        /// <returns></returns>
        private static List<List<Card>> FindLongestSequenceMelds(List<List<Card>> handSortedBySuit)
        {
            List<List<Card>> longestSequenceMelds = new();

            // Loop through each card suit grouping and pick out the longest length sequence melds for each suit
            foreach (var suitCards in handSortedBySuit)
            {
                var melds = ExtractLongestSequence(suitCards);

                foreach(var meld in melds)
                {
                    if (meld.Count >= 3) longestSequenceMelds.Add(meld);
                }
            }

            return longestSequenceMelds;
        }

        /// <summary>
        /// Return a list of the longest possible sequence melds that can be obtained from a particular suit of the player's hand.
        /// </summary>
        /// <param name="sameSuitCards"></param>
        /// <returns></returns>
        private static List<List<Card>> ExtractLongestSequence(List<Card> sameSuitCards)
        {
            List<List<Card>> melds = new();

            // Minimum meld size is 3 cards so only continue the loop while atleast 3 cards still remain
            while (sameSuitCards.Count >= 3)
            {
                // Take the 1st card in the list. If it does not make a sequence with the next 2 cards, remove it from the list 
                if (sameSuitCards[0].Rank != (sameSuitCards[1].Rank - 1) ||
                    sameSuitCards[0].Rank != (sameSuitCards[2].Rank - 2))
                {
                    sameSuitCards.Remove(sameSuitCards.First());
                }
                else
                {
                    List<Card> meld = new();

                    // If the first 3 cards in the list make a sequence, add them to the meld variable then remove them from the list
                    meld = sameSuitCards.Take(3).ToList();
                    sameSuitCards.RemoveRange(0, 3);

                    // Check the remaining cards in the list. If they are also part of the sequence, add them to meld variable and remove from the list.
                    // Continue this loop while there is atleast 1 card left and until a card that is not in sequence is found
                    while (sameSuitCards.Count > 0)
                    {
                        if (sameSuitCards.First().Rank == (meld.Last().Rank + 1))
                        {
                            meld.Add(sameSuitCards.First());
                            sameSuitCards.Remove(sameSuitCards.First());
                        }
                        else break;
                    }

                    melds.Add(meld);
                }                
            }

            return melds;
        }

        /// <summary>
        /// Return a readable string of all 10 cards that are in the player's hand.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
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
