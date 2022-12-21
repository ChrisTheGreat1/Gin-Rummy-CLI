// See https://aka.ms/new-console-template for more information
using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Helpers;
using static _11242022_Gin_Rummy.Helpers.DeckMethods;
using static _11242022_Gin_Rummy.Helpers.HandMethods;
using static _11242022_Gin_Rummy.Helpers.GameLogicMethods;
using _11242022_Gin_Rummy.Models;
using System;
using System.Text;
using static System.Console;


const int HAND_SIZE = 10;

List<Card> deck = CreateDeck();
List<Card> discardPile = new List<Card>();

List<Card> handPlayerOne = new List<Card>();
List<Card> handPlayerTwo = new List<Card>();
handPlayerOne.Capacity = HAND_SIZE;
handPlayerTwo.Capacity = HAND_SIZE;

Card pickedUpCard = new Card();

char userInput = ' ';

bool isPlayerOneTurn;
bool isGameOver = false;
bool canPlayerOneKnock = false;
bool canPlayerTwoKnock = false;

int handPlayerOneValue;
int handPlayerTwoValue;
int playerOneScore = 0;
int playerTwoScore = 0;

ShuffleDeck(deck);

DealOutHands();

SortHandsDetectMelds();
DetermineIfKnockingEligible();

isPlayerOneTurn = DetermineDealer(); // If assigned true, player one is dealer. Otherwise player two is dealer.

PrintHandsToConsole();

FirstTurnChanceToPickupFromDiscardPile();

userInput = ' ';

// -------------------------------------------------------------------------------------------------------------------------------------------
// TODO: player who won previous game becomes dealer

// TODO: scoring code - First to get to 100 or more wins

// TODO: update all methods so that input argument isn't directly modified
// -------------------------------------------------------------------------------------------------------------------------------------------

// TODO: colour coding for hand displays (also melds?)


while (deck.Count > 0 && !isGameOver)
{
    PrintHandsToConsole();

    WriteLine(CurrentPlayerString(isPlayerOneTurn) + " - Press 'd' if you wish to pick up from the discard pile," +
        " or 'n' if you wish to pick up a new card from the deck.\n");

    // TODO: hotkey for displaying hand history
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
                SortHandsDetectMelds();
                DetectIfGinHasOccurred();
                DetermineIfKnockingEligible();
                PromptPlayerToKnock();
                break;
            case 'h':
            case 'H':
                discardPile.Add(pickedUpCard);
                WriteLine("\n" + CurrentPlayerString(isPlayerOneTurn) + " discarded " + discardPile.Last().ToString() + "\n");

                SortHandsDetectMelds();
                DetectIfGinHasOccurred();
                DetermineIfKnockingEligible();
                PromptPlayerToKnock();
                break;
            default:
                WriteLine("\nInvalid input.\n");
                break;
        }
    }

    userInput = ' ';
    isPlayerOneTurn = !isPlayerOneTurn;
}

if(deck.Count == 0)
{
    WriteLine("\nEnd of deck reached - tallying points remaining in player hands.\n");

    handPlayerOneValue = CalculateHandValue(handPlayerOne);
    handPlayerTwoValue = CalculateHandValue(handPlayerTwo);

    playerOneScore += handPlayerTwoValue;
    playerTwoScore += handPlayerOneValue;
}

WriteLine("End of game reached.\n");
PrintHandsToConsole();

// TODO: method to display score of hand

// TODO: refactor so hands are always in same position of console. Clear console of old log messages
void PrintHandsToConsole()
{
    handPlayerOne = SortHandWithMeldGroupings(handPlayerOne);
    handPlayerTwo = SortHandWithMeldGroupings(handPlayerTwo);

    WriteLine("---------------------------------------");
    if (canPlayerTwoKnock) WriteLine("****PLAYER TWO CAN KNOCK****");
    WriteLine(HandToString(handPlayerTwo));
    WriteLine("\nDiscard pile: " + discardPile.Last().ToString() + "\n");
    WriteLine(HandToString(handPlayerOne));
    if (canPlayerOneKnock) WriteLine("****PLAYER ONE CAN KNOCK****");
    WriteLine("---------------------------------------");
    WriteLine("0  1  2  3  4  5  6  7  8  9\n");
}

void DiscardFromHand(bool isPlayerOneTurn, int userInput)
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

void DetectIfGinHasOccurred()
{
    if (isPlayerOneTurn)
    {
        if (CalculateHandValue(handPlayerOne) == 0)
        {
            playerOneScore += 20;
            playerOneScore += CalculateHandValue(handPlayerTwo);
            isGameOver = true;
        }
    }
    else
    {
        if (CalculateHandValue(handPlayerTwo) == 0)
        {
            playerTwoScore += 20;
            playerTwoScore += CalculateHandValue(handPlayerOne);
            isGameOver = true;
        }
    }
}

void FirstTurnChanceToPickupFromDiscardPile()
{
    if (canPlayerOneKnock || canPlayerTwoKnock)
    {
        WriteLine("\nMISDEAL - atleast one player can knock before any cards have been exchanged.\n");
        isGameOver = true;
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
        WriteLine("\nNon-dealer chose to pass - dealer now has chance to pick up card from discard pile.\n");
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

void PromptPlayerToKnock()
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

void NonKnockerCombinesUnmatchedCardsWithKnockersMelds()
{
    if (isPlayerOneTurn) handPlayerTwo = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handPlayerOne, handPlayerTwo);
    else handPlayerOne = HandMethods.NonKnockerCombinesUnmatchedCardsWithKnockersMelds(handPlayerTwo, handPlayerOne);
}

void UpdatePlayerScoresAfterKnocking()
{
    handPlayerOneValue = CalculateHandValue(handPlayerOne);
    handPlayerTwoValue = CalculateHandValue(handPlayerTwo);

    int points = handPlayerOneValue - handPlayerTwoValue;

    if(isPlayerOneTurn)
    {
        if (points == 0)
        {
            playerTwoScore += 10;
            return;
        }

        if (points < 0) playerOneScore += Math.Abs(points);

        else
        {
            playerTwoScore += Math.Abs(points);
            playerTwoScore += 10;
        }
    }
    
    else
    {
        if (points == 0)
        {
            playerOneScore += 10;
            return; 
        }

        if (points > 0) playerTwoScore += Math.Abs(points);

        else
        {
            playerOneScore += Math.Abs(points);
            playerOneScore += 10;
        }
    }
}



void DetermineIfKnockingEligible()
{
    if (isGameOver) return;

    canPlayerOneKnock = CanPlayerKnock(handPlayerOne);
    canPlayerTwoKnock = CanPlayerKnock(handPlayerTwo);
}

void SortHandsDetectMelds()
{
    handPlayerOne = DetectAndGroupByMelds(handPlayerOne);
    handPlayerTwo = DetectAndGroupByMelds(handPlayerTwo);
}

void DealOutHands()
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