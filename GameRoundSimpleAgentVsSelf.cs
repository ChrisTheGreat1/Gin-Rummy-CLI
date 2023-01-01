using _11242022_Gin_Rummy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Helpers;
using static _11242022_Gin_Rummy.Helpers.DeckMethods;
using static _11242022_Gin_Rummy.Helpers.HandMethods;
using static _11242022_Gin_Rummy.Helpers.GameLogicMethods;
using static System.Console;

namespace _11242022_Gin_Rummy
{
    public static class GameRoundSimpleAgentVsSelf
    {
        private const int HAND_SIZE = 10;

        private static List<Card> deck;
        private static List<Card> discardPile;

        private static List<Card> handPlayerOne; // Human player
        private static List<Card> handPlayerTwo; // Simple computer player

        private static Card pickedUpCard;

        private static char userInput = ' ';

        private static bool isPlayerOneTurn;
        private static bool isGameOver = false;
        private static bool canPlayerOneKnock = false;
        private static bool canPlayerTwoKnock = false;

        private static int handPlayerOneValue;
        private static int handPlayerTwoValue;
        private static int playerOneRoundScore;
        private static int playerTwoRoundScore;
        private static int winnerNumber = 0;

        public static List<int> PlayRound(int previousWinner = 0)
        {
            // Reset values to prevent interference from previous game
            userInput = ' ';
            handPlayerOneValue = 0;
            handPlayerTwoValue = 0;
            playerOneRoundScore = 0;
            playerTwoRoundScore = 0;
            canPlayerOneKnock = false;
            canPlayerTwoKnock = false;
            isGameOver = false;
            discardPile = new();
            handPlayerOne = new();
            handPlayerTwo = new();
            pickedUpCard = new();
            deck = new();

            deck = CreateDeck();

            deck = ShuffleDeck(deck);

            DealOutHands();

            SortHandsDetectMelds();
            DetermineIfKnockingEligible();

            if (previousWinner == 1) isPlayerOneTurn = true;
            else if (previousWinner == 2) isPlayerOneTurn = false;
            else isPlayerOneTurn = DetermineDealer();

            PrintHandsToConsole();

            FirstTurnChanceToPickupFromDiscardPile();

            userInput = ' ';

            while (deck.Count > 0 && !isGameOver)
            {
                PrintHandsToConsole();

                //if (isPlayerOneTurn)
                //{
                //    HumanPlayerChooseWhereToPickUpFrom();

                //    HumanPlayerChooseDiscard();
                //}
                //else
                //{
                    SimpleAgentPlaysHandVsSelf(isPlayerOneTurn);
                //}

                isPlayerOneTurn = !isPlayerOneTurn;
            }

            if (deck.Count == 0)
            {
                WriteLine("\nEnd of deck reached - tallying points remaining in player hands.\n");

                handPlayerOneValue = CalculateHandValue(handPlayerOne);
                handPlayerTwoValue = CalculateHandValue(handPlayerTwo);

                playerOneRoundScore += handPlayerTwoValue;
                playerTwoRoundScore += handPlayerOneValue;
            }

            WriteLine("End of round reached.\n");
            PrintHandsToConsole();
            PrintScoresToConsole();

            WriteLine("Press any key to continue.\n");
            //ReadKey();

            List<int> endOfGameInfo = new();
            endOfGameInfo.Add(playerOneRoundScore);
            endOfGameInfo.Add(playerTwoRoundScore);
            endOfGameInfo.Add(winnerNumber);

            return endOfGameInfo;
        }

        private static void SimpleAgentPlaysHandVsSelf(bool isPlayerOneTurn)
        {
            var discardPileCard = discardPile.Last();

            List<Card> _hand = new List<Card>();

            if (isPlayerOneTurn)
            {
                _hand = handPlayerOne;
            }
            else
            {
                _hand = handPlayerTwo;
            }

            _hand.Add(discardPileCard);
            _hand = DetermineMeldsInHand(_hand);

            var nonMeldedCards = _hand.Where(c => !c.IsInMeld).ToList();

            // If hand is in gin, remove a card from the players hand
            if (nonMeldedCards.Count == 0)
            {
                var groupedMelds = _hand.GroupBy(c => c.MeldGroupIdentifier).ToList();

                var largestMeldGroup = groupedMelds.Where(m => (m.Count() > 3)).First(); // Find a meld with more than 3 cards in it

                _hand.Remove(largestMeldGroup.Last());

                return;
            }

            // If card from discard pile doesn't form a meld, pick up a card from the deck
            if (nonMeldedCards.Contains(discardPileCard))
            {
                _hand.Remove(discardPileCard);

                pickedUpCard = deck.Last();
                WriteLine(CurrentPlayerString(isPlayerOneTurn) + " has chosen to pick up a card from the deck.\n");
                WriteLine(CurrentPlayerString(isPlayerOneTurn) + " picked up " + pickedUpCard.ToString() + "\n");

                deck.Remove(deck.Last());

                _hand.Add(pickedUpCard);
                _hand = DetermineMeldsInHand(_hand);

                nonMeldedCards = _hand.Where(c => !c.IsInMeld).ToList();

                // If hand is in gin, remove a card from the players hand
                if (nonMeldedCards.Count == 0)
                {
                    var groupedMelds = _hand.GroupBy(c => c.MeldGroupIdentifier).ToList();

                    var largestMeldGroup = groupedMelds.Where(m => (m.Count() > 3)).First(); // Find a meld with more than 3 cards in it

                    _hand.Remove(largestMeldGroup.Last());

                    return;
                }

                var highestDeadwoodCard = nonMeldedCards.OrderByDescending(c => c.Rank).First();

                _hand.Remove(highestDeadwoodCard);
                discardPile.Add(highestDeadwoodCard);

                WriteLine(CurrentPlayerString(isPlayerOneTurn) + " discarded " + highestDeadwoodCard.ToString() + "\n");
            }

            // If card did complete a meld, discard the highest deadwood value non-melded card remaining in hand
            else
            {
                var highestDeadwoodCard = nonMeldedCards.OrderByDescending(c => c.Rank).First();

                _hand.Remove(highestDeadwoodCard);
                discardPile.Remove(discardPileCard);
                discardPile.Add(highestDeadwoodCard);

                WriteLine(CurrentPlayerString(isPlayerOneTurn) + " picked up " + discardPileCard.ToString() + " from the discard pile.\n");
                WriteLine(CurrentPlayerString(isPlayerOneTurn) + " discarded " + highestDeadwoodCard.ToString() + "\n");
            }

            if (isPlayerOneTurn)
            {
                handPlayerOne = _hand;
            }
            else
            {
                handPlayerTwo = _hand;
            }

            // TODO: consider optimizing by removing unnecessary hand method calls when it's not the players turn
            DetectIfGinHasOccurred();
            DetermineIfKnockingEligible();
            PromptPlayerToKnock();
        }

        private static void HumanPlayerChooseDiscard()
        {
            WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " - Enter number 0-9 to select card from hand to discard," +
                " or press 'h' to discard newly picked up card.\n");

            while (userInput != '0' && userInput != '1' && userInput != '2' && userInput != '3'
                                    && userInput != '4' && userInput != '5' && userInput != '6' && userInput != '7'
                                    && userInput != '8' && userInput != '9' && userInput != 'h' && userInput != 'H')
            {
                userInput = ReadKey().KeyChar;
                WriteLine();

                switch (userInput)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        DiscardFromHand(isPlayerOneTurn, (int)Char.GetNumericValue(userInput));
                        break;
                    case 'h':
                    case 'H':
                        discardPile.Add(pickedUpCard);
                        WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " discarded " + discardPile.Last().ToString() + "\n");
                        break;
                    default:
                        WriteLine("\nInvalid input.\n");
                        break;
                }

                SortHandsDetectMelds();
                DetectIfGinHasOccurred();
                DetermineIfKnockingEligible();
                PromptPlayerToKnock();
            }

            userInput = ' ';
        }

        private static void HumanPlayerChooseWhereToPickUpFrom()
        {
            WriteLine(CurrentPlayerString(isPlayerOneTurn) + " - Press 'd' if you wish to pick up from the discard pile," +
                " or 'n' if you wish to pick up a new card from the deck.\n");

            while (userInput != 'd' && userInput != 'D' && userInput != 'n' && userInput != 'N')
            {
                userInput = ReadKey().KeyChar;
                WriteLine();

                switch (userInput)
                {
                    case 'd':
                    case 'D':
                        pickedUpCard = discardPile.Last();
                        discardPile.Remove(discardPile.Last());

                        WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " picked up " + pickedUpCard.ToString());
                        break;
                    case 'n':
                    case 'N':
                        pickedUpCard = deck.Last();
                        deck.Remove(deck.Last());

                        WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " picked up " + pickedUpCard.ToString());
                        break;
                    default:
                        WriteLine("\nInvalid input.\n");
                        break;
                }
            }

            userInput = ' ';
        }

        private static void PrintScoresToConsole()
        {
            WriteLine("Player one round score: " + playerOneRoundScore);
            WriteLine("Player two round score: " + playerTwoRoundScore);
        }

        private static void PrintHandsToConsole()
        {
            handPlayerOne = SortHandWithMeldGroupings(handPlayerOne);
            handPlayerTwo = SortHandWithMeldGroupings(handPlayerTwo);

            //Clear();

            WriteLine("---------------------------------------");
            if (canPlayerTwoKnock) WriteLine("****PLAYER TWO CAN KNOCK****");
            WriteLine(HandToString(handPlayerTwo));
            WriteLine("\nDiscard pile: " + discardPile.Last().ToString() + "\n");
            WriteLine(HandToString(handPlayerOne));
            if (canPlayerOneKnock) WriteLine("****PLAYER ONE CAN KNOCK****");
            WriteLine("---------------------------------------");
            WriteLine("0  1  2  3  4  5  6  7  8  9\n");
        }

        private static void DiscardFromHand(bool isPlayerOneTurn, int userInput)
        {
            if (isPlayerOneTurn)
            {
                discardPile.Add(handPlayerOne[userInput]);
                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " discarded " + discardPile.Last().ToString() + "\n");
                handPlayerOne[userInput] = pickedUpCard;
            }
            else
            {
                discardPile.Add(handPlayerTwo[userInput]);
                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " discarded " + discardPile.Last().ToString() + "\n");
                handPlayerTwo[userInput] = pickedUpCard;
            }
        }

        private static void DetectIfGinHasOccurred()
        {
            if (isPlayerOneTurn)
            {
                if (DetectGin(handPlayerOne))
                {
                    playerOneRoundScore += 20;
                    playerOneRoundScore += CalculateHandValue(handPlayerTwo);
                    winnerNumber = 1;
                    isGameOver = true;
                }
            }
            else
            {
                if (DetectGin(handPlayerTwo))
                {
                    playerTwoRoundScore += 20;
                    playerTwoRoundScore += CalculateHandValue(handPlayerOne);
                    winnerNumber = 2;
                    isGameOver = true;
                }
            }
        }

        private static void FirstTurnChanceToPickupFromDiscardPile()
        {
            if (canPlayerOneKnock || canPlayerTwoKnock)
            {
                WriteLine("\nMISDEAL - atleast one player can knock before any cards have been exchanged.\n");
                isGameOver = true;
                winnerNumber = 0;
                return;
            }

            isPlayerOneTurn = !isPlayerOneTurn;
            WriteLine(CurrentPlayerString(isPlayerOneTurn) + " (NON-DEALER) - Press 'd' if you wish to pick up from the discard pile, or 'n' if you wish to pass without discarding.\n");

            bool didNonDealerPickupAtFirstChance = false;
            //bool breakSwitch = false;

            OfferChanceToPickUpFirstCardFromDiscardPile();

            isPlayerOneTurn = !isPlayerOneTurn;
            userInput = ' ';

            // If non-dealer passed up first chance at discard pile, dealer is given chance to pickup the card
            if (!didNonDealerPickupAtFirstChance)
            {
                WriteLine("Non-dealer chose to pass - dealer now has chance to pick up card from discard pile.\n");
                WriteLine(CurrentPlayerString(isPlayerOneTurn) + " - Press 'd' if you wish to pick up from the discard pile, or 'n' if you wish to pass.\n");

                OfferChanceToPickUpFirstCardFromDiscardPile();

                isPlayerOneTurn = !isPlayerOneTurn;
            }

            SortHandsDetectMelds();
            DetermineIfKnockingEligible();

            void OfferChanceToPickUpFirstCardFromDiscardPile()
            {
                //if (isPlayerOneTurn)
                //{
                //    while (userInput != 'd' && userInput != 'D' && userInput != 'n' && userInput != 'N' && breakSwitch == false)
                //    {
                //        userInput = ReadKey().KeyChar;
                //        WriteLine();

                //        switch (userInput)
                //        {
                //            case 'd':
                //            case 'D':
                //                pickedUpCard = discardPile.Last();
                //                discardPile.Remove(discardPile.Last());

                //                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " picked up " + pickedUpCard.ToString());

                //                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " - Enter number 0-9 to select card from hand to discard.\n");

                //                while (userInput != '0' && userInput != '1' && userInput != '2' && userInput != '3'
                //                    && userInput != '4' && userInput != '5' && userInput != '6' && userInput != '7'
                //                    && userInput != '8' && userInput != '9')
                //                {
                //                    userInput = ReadKey().KeyChar;
                //                    WriteLine();

                //                    switch (userInput)
                //                    {
                //                        case '0':
                //                        case '1':
                //                        case '2':
                //                        case '3':
                //                        case '4':
                //                        case '5':
                //                        case '6':
                //                        case '7':
                //                        case '8':
                //                        case '9':
                //                            DiscardFromHand(isPlayerOneTurn, (int)Char.GetNumericValue(userInput));
                //                            didNonDealerPickupAtFirstChance = true;
                //                            breakSwitch = true;
                //                            break;
                //                        default:
                //                            WriteLine("\nInvalid input.\n");
                //                            break;
                //                    }
                //                }
                //                break;

                //            case 'n':
                //            case 'N':
                //                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " has chosen to pass.\n");
                //                break;
                //            default:
                //                WriteLine("\nInvalid input.\n");
                //                break;
                //        }
                //    }
                //}

                //else
                //{
                didNonDealerPickupAtFirstChance = SimpleAgent_DetermineWhetherToPickupFirstCardFromDiscardPile(isPlayerOneTurn);
                //}
            }
        }

        private static bool SimpleAgent_DetermineWhetherToPickupFirstCardFromDiscardPile(bool isPlayerOneTurn)
        {
            bool didNonDealerPickupAtFirstChance;
            var discardPileCard = discardPile.Last();

            List<Card> _hand = new List<Card>();

            if (isPlayerOneTurn)
            {
                _hand = handPlayerOne;
            }
            else
            {
                _hand = handPlayerTwo;
            }

            _hand.Add(discardPileCard);
            _hand = DetermineMeldsInHand(_hand);

            var nonMeldedCards = _hand.Where(c => !c.IsInMeld).ToList();
            var highestDeadwoodCard = nonMeldedCards.OrderByDescending(c => c.Rank).First();

            if (nonMeldedCards.Contains(discardPileCard))
            {
                if (highestDeadwoodCard == discardPileCard)
                {
                    _hand.Remove(discardPileCard);
                    didNonDealerPickupAtFirstChance = false;

                    WriteLine(CurrentPlayerString(isPlayerOneTurn) + " has chosen to pass.\n");
                }
                else
                {
                    _hand.Remove(highestDeadwoodCard);
                    discardPile.Remove(discardPileCard);
                    discardPile.Add(highestDeadwoodCard);

                    didNonDealerPickupAtFirstChance = true;

                    WriteLine(CurrentPlayerString(isPlayerOneTurn) + " picked up " + discardPileCard.ToString() + "\n");
                }
            }
            else
            {
                _hand.Remove(highestDeadwoodCard);
                discardPile.Remove(discardPileCard);
                discardPile.Add(highestDeadwoodCard);

                didNonDealerPickupAtFirstChance = true;

                WriteLine(CurrentPlayerString(isPlayerOneTurn) + " picked up " + discardPileCard.ToString() + "\n");
            }

            if (isPlayerOneTurn)
            {
                handPlayerOne = _hand;
            }
            else
            {
                handPlayerTwo = _hand;
            }

            return didNonDealerPickupAtFirstChance;
        }

        private static void PromptPlayerToKnock()
        {
            if (isGameOver) return;
            if ((isPlayerOneTurn && !canPlayerOneKnock) || (!isPlayerOneTurn && !canPlayerTwoKnock)) return;

            //WriteLine(CurrentPlayerString(isPlayerOneTurn) + " can knock (hand value less than 10 points) " +
            //    "- press 'k' if you wish to knock, or 'd' if you wish to continue playing.\n");

            //char userKnockInput = ' ';

            //if (isPlayerOneTurn)
            //{
            //    while (userKnockInput != 'k' && userKnockInput != 'K' && userKnockInput != 'd' && userKnockInput != 'D')
            //    {
            //        userKnockInput = ReadKey().KeyChar;
            //        WriteLine();

            //        switch (userKnockInput)
            //        {
            //            case 'k':
            //            case 'K':
            //                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " has chosen to knock and end the game.\n");
            //                isGameOver = true;
            //                NonKnockerCombinesUnmatchedCardsWithKnockersMelds();
            //                UpdatePlayerScoresAfterKnocking();
            //                break;
            //            case 'd':
            //            case 'D':
            //                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " has chosen to continue playing.\n");
            //                break;
            //            default:
            //                WriteLine("\nInvalid input.\n");
            //                break;
            //        }
            //    }
            //}
            //else
            //{
                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " has chosen to knock and end the game.\n");
                isGameOver = true;
                NonKnockerCombinesUnmatchedCardsWithKnockersMelds();
                UpdatePlayerScoresAfterKnocking();
            //}
        }

        private static void NonKnockerCombinesUnmatchedCardsWithKnockersMelds()
        {
            if (isPlayerOneTurn) handPlayerTwo = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handPlayerOne, handPlayerTwo);
            else handPlayerOne = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handPlayerTwo, handPlayerOne);
        }

        private static void UpdatePlayerScoresAfterKnocking()
        {
            handPlayerOneValue = CalculateHandValue(handPlayerOne);
            handPlayerTwoValue = CalculateHandValue(handPlayerTwo);

            int points = handPlayerOneValue - handPlayerTwoValue;

            if (isPlayerOneTurn)
            {
                if (points == 0)
                {
                    playerTwoRoundScore += 10;
                    winnerNumber = 2;
                    return;
                }

                if (points < 0)
                {
                    playerOneRoundScore += Math.Abs(points);
                    winnerNumber = 1;
                }

                else
                {
                    playerTwoRoundScore += Math.Abs(points);
                    playerTwoRoundScore += 10;
                    winnerNumber = 2;
                }
            }

            else
            {
                if (points == 0)
                {
                    playerOneRoundScore += 10;
                    winnerNumber = 1;
                    return;
                }

                if (points > 0)
                {
                    playerTwoRoundScore += Math.Abs(points);
                    winnerNumber = 2;
                }

                else
                {
                    playerOneRoundScore += Math.Abs(points);
                    playerOneRoundScore += 10;
                    winnerNumber = 1;
                }
            }
        }

        private static void DetermineIfKnockingEligible()
        {
            if (isGameOver) return;

            canPlayerOneKnock = CanPlayerKnock(handPlayerOne);
            canPlayerTwoKnock = CanPlayerKnock(handPlayerTwo);
        }

        private static void SortHandsDetectMelds()
        {
            handPlayerOne = DetermineMeldsInHand(handPlayerOne);
            handPlayerTwo = DetermineMeldsInHand(handPlayerTwo);
        }

        private static void DealOutHands()
        {
            for (int i = 0; i < HAND_SIZE; i++)
            {
                handPlayerOne.Add(deck.Last());
                deck.Remove(deck.Last());

                handPlayerTwo.Add(deck.Last());
                deck.Remove(deck.Last());
            }

            discardPile.Add(deck.Last());
            deck.Remove(deck.Last());
        }
    }
}
