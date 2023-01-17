using _11242022_Gin_Rummy.Helpers;
using _11242022_Gin_Rummy.Models;
using static _11242022_Gin_Rummy.Helpers.DeckMethods;
using static _11242022_Gin_Rummy.Helpers.GameLogicMethods;
using static _11242022_Gin_Rummy.Helpers.HandMethods;
using static System.Console;

namespace _11242022_Gin_Rummy
{
    public static class GameRound
    {
        private const int HAND_SIZE = 10;

        private static bool canPlayerOneKnock = false;
        private static bool canPlayerTwoKnock = false;
        private static bool isGameOver = false;
        private static bool isPlayerOneTurn;

        private static Card pickedUpCard;

        private static List<Card> deck;
        private static List<Card> discardPile;
        private static List<Card> handPlayerOne; 
        private static List<Card> handPlayerTwo; 

        private static int handPlayerOneValue;
        private static int handPlayerTwoValue;
        private static int playerOneRoundScore;
        private static int playerTwoRoundScore;
        private static int winnerNumber = 0;

        private static char userInput = ' ';

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

                HumanPlayerChooseWhereToPickUpFrom();

                HumanPlayerChooseDiscard();

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

            WriteLine("Press any key to continue.");
            ReadKey();

            List<int> endOfGameInfo = new();
            endOfGameInfo.Add(playerOneRoundScore);
            endOfGameInfo.Add(playerTwoRoundScore);
            endOfGameInfo.Add(winnerNumber);

            return endOfGameInfo;
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

        private static void DetermineIfKnockingEligible()
        {
            if (isGameOver) return;

            canPlayerOneKnock = CanPlayerKnock(handPlayerOne);
            canPlayerTwoKnock = CanPlayerKnock(handPlayerTwo);
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
            bool breakSwitch = false;

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
                while (userInput != 'd' && userInput != 'D' && userInput != 'n' && userInput != 'N' && breakSwitch == false)
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

                            WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " - Enter number 0-9 to select card from hand to discard.\n");

                            while (userInput != '0' && userInput != '1' && userInput != '2' && userInput != '3'
                                && userInput != '4' && userInput != '5' && userInput != '6' && userInput != '7'
                                && userInput != '8' && userInput != '9')
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
                                        didNonDealerPickupAtFirstChance = true;
                                        breakSwitch = true;
                                        break;

                                    default:
                                        WriteLine("\nInvalid input.\n");
                                        break;
                                }
                            }
                            break;

                        case 'n':
                        case 'N':
                            WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " has chosen to pass.\n");
                            break;

                        default:
                            WriteLine("\nInvalid input.\n");
                            break;
                    }
                }
            }
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

        private static void NonKnockerCombinesUnmatchedCardsWithKnockersMelds()
        {
            if (isPlayerOneTurn) handPlayerTwo = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handPlayerOne, handPlayerTwo);
            else handPlayerOne = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handPlayerTwo, handPlayerOne);
        }

        private static void PrintHandsToConsole()
        {
            handPlayerOne = SortHandWithMeldGroupings(handPlayerOne);
            handPlayerTwo = SortHandWithMeldGroupings(handPlayerTwo);

            Clear();

            WriteLine("---------------------------------------");
            if (canPlayerTwoKnock) WriteLine("****PLAYER TWO CAN KNOCK****");
            WriteLine(HandToString(handPlayerTwo));
            WriteLine("\nDiscard pile: " + discardPile.Last().ToString() + "\n");
            WriteLine(HandToString(handPlayerOne));
            if (canPlayerOneKnock) WriteLine("****PLAYER ONE CAN KNOCK****");
            WriteLine("---------------------------------------");
            WriteLine("0  1  2  3  4  5  6  7  8  9\n");
        }

        private static void PrintScoresToConsole()
        {
            WriteLine("Player one round score: " + playerOneRoundScore);
            WriteLine("Player two round score: " + playerTwoRoundScore);
        }
        private static void PromptPlayerToKnock()
        {
            if (isGameOver) return;
            if ((isPlayerOneTurn && !canPlayerOneKnock) || (!isPlayerOneTurn && !canPlayerTwoKnock)) return;

            WriteLine(CurrentPlayerString(isPlayerOneTurn) + " can knock (hand value less than 10 points) " +
                "- press 'k' if you wish to knock, or 'd' if you wish to continue playing.\n");

            char userKnockInput = ' ';

            while (userKnockInput != 'k' && userKnockInput != 'K' && userKnockInput != 'd' && userKnockInput != 'D')
            {
                userKnockInput = ReadKey().KeyChar;
                WriteLine();

                switch (userKnockInput)
                {
                    case 'k':
                    case 'K':
                        WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " has chosen to knock and end the game.\n");
                        isGameOver = true;
                        NonKnockerCombinesUnmatchedCardsWithKnockersMelds();
                        UpdatePlayerScoresAfterKnocking();
                        break;

                    case 'd':
                    case 'D':
                        WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " has chosen to continue playing.\n");
                        break;

                    default:
                        WriteLine("\nInvalid input.\n");
                        break;
                }
            }
        }
        private static void SortHandsDetectMelds()
        {
            handPlayerOne = DetermineMeldsInHand(handPlayerOne);
            handPlayerTwo = DetermineMeldsInHand(handPlayerTwo);
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
    }
}