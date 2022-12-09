// See https://aka.ms/new-console-template for more information
using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Helpers;
using _11242022_Gin_Rummy.Models;
using System;
using System.Text;
using static System.Console;

const int HAND_SIZE = 10;

var deck = DeckMethods.CreateDeck();
var discardPile = new List<Card>();
var pickedUpCard = new Card();
char userInput = ' ';
bool isPlayerOneTurn;
bool didNonDealerPickupAtFirstChance = false;
bool breakSwitch = false;

deck = DeckMethods.ShuffleDeck(deck);

var handPlayerOne = new List<Card>();
handPlayerOne.Capacity = 10;

var handPlayerTwo = new List<Card>();
handPlayerTwo.Capacity = 10;

// Deal hand
for (int i = 0; i < HAND_SIZE; i++)
{
    handPlayerOne.Add(deck.Last());
    deck.Remove(deck.Last());

    handPlayerTwo.Add(deck.Last());
    deck.Remove(deck.Last());
}

discardPile.Add(deck.Last());
deck.Remove(deck.Last());

handPlayerOne = HandMethods.DetectAndGroupByMelds(handPlayerOne);
handPlayerTwo = HandMethods.DetectAndGroupByMelds(handPlayerTwo);

isPlayerOneTurn = DetermineDealer(); // If assigned true, player one is dealer. Otherwise player two is dealer.

PrintHandsToConsole();

if (isPlayerOneTurn)
{
    WriteLine("PLAYER TWO (NON-DEALER) - Press 'd' if you wish to pick up from the discard pile, or 'n' if you wish to pass without discarding.");
    WriteLine();
}
else
{
    WriteLine("PLAYER ONE (NON-DEALER) - Press 'd' if you wish to pick up from the discard pile, or 'n' if you wish to pass without discarding.");
    WriteLine();
}

isPlayerOneTurn = !isPlayerOneTurn;

// TODO: cancel 2nd player from going if 1st went
// First player's turn
FirstTurnChanceToPickupFromDiscardPile();

userInput = ' ';

// -------------------------------------------------------------------------------------------------------------------------------------------
// TODO: player who won previous game becomes dealer

// TODO: If the stock has run out and the next player does not want to take the discard,
// the game ends at that point. Everyone scores the value of the cards remaining in their hands.

// TODO: logic to prevent player from picking up card that they just discarded

// TODO: logic to detect when player can knock (10 points or less)
// TODO: logic for non-knocking player to combine cards with opponents hand after they lay down (need logic to also ensure 5-card runs can't be added onto)
// TODO: determine winner when knock happens

// TODO: scoring code
// If the player who Knocks wins the game, they score the difference in the value of their unmatched cards with those
// of their opponent, while if the opponent wins, they score 10 points plus the difference in the value of the unmatched
// cards between both players. If there is no difference, the 10 point bonus remains.
// First to get to 100 or more wins

// TODO: logic to detect when all cards in hand are in a meld (gin!). auto-end game
// TODO: 20 point bonus to winner for gin
// -------------------------------------------------------------------------------------------------------------------------------------------

// TODO: colour coding for hand displays (also melds?)


while (deck.Count > 0)
{
    PrintHandsToConsole();

    WriteLine(CurrentPlayerString() + " - Press 'd' if you wish to pick up from the discard pile, or 'n' if you wish to pick up a new card from the deck.");
    WriteLine();
    WriteLine();

    // TODO: hotkey for displaying hand history
    while (userInput != 'd' && userInput != 'n')
    {
        userInput = ReadKey().KeyChar;

        switch (userInput)
        {
            case 'd':
                pickedUpCard = discardPile.Last();
                discardPile.Remove(discardPile.Last());
                WriteLine();
                WriteLine();
                WriteLine("Picked up " + pickedUpCard.ToString());
                break;
            case 'n':
                pickedUpCard = deck.Last();
                deck.Remove(deck.Last());
                WriteLine();
                WriteLine();
                WriteLine("Picked up " + pickedUpCard.ToString());
                break;
            default:
                WriteLine();
                WriteLine("Invalid input.\n");
                break;
        }
    }

    userInput = ' ';
    WriteLine();
    WriteLine("Enter number 0-9 to select card from hand to discard, or press 'h' to discard newly picked up card.");
    WriteLine();

    while (userInput != '0' && userInput != '1' && userInput != '2' && userInput != '3'
        && userInput != '4' && userInput != '5' && userInput != '6' && userInput != '7'
        && userInput != '8' && userInput != '9' && userInput != 'h')
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
                discardPile.Add(pickedUpCard);
                WriteLine();
                WriteLine("Discarded " + discardPile.Last().ToString());
                break;
            default:
                WriteLine();
                WriteLine("Invalid input.\n");
                break;
        }

        handPlayerOne = HandMethods.DetectAndGroupByMelds(handPlayerOne);
        handPlayerTwo = HandMethods.DetectAndGroupByMelds(handPlayerTwo);
    }

    userInput = ' ';
    isPlayerOneTurn = !isPlayerOneTurn;
}

WriteLine();
WriteLine("End of game reached.");

void PrintHandsToConsole()
{
    handPlayerOne = HandMethods.SortHandWithMeldGroupings(handPlayerOne);
    handPlayerTwo = HandMethods.SortHandWithMeldGroupings(handPlayerTwo);

    WriteLine();
    WriteLine(HandMethods.HandToString(handPlayerTwo));
    WriteLine();
    WriteLine("Discard pile: " + discardPile.Last().ToString());
    WriteLine();
    WriteLine(HandMethods.HandToString(handPlayerOne));
    WriteLine("0  1  2  3  4  5  6  7  8  9");
    WriteLine();
}

void DiscardFromHand(bool isPlayerOneTurn, int userInput)
{
    if (isPlayerOneTurn)
    {
        discardPile.Add(handPlayerOne[userInput]);
        WriteLine();
        WriteLine("Discarded " + discardPile.Last().ToString());
        handPlayerOne[userInput] = pickedUpCard;
    }
    else
    {
        discardPile.Add(handPlayerTwo[userInput]);
        WriteLine();
        WriteLine("Discarded " + discardPile.Last().ToString());
        handPlayerTwo[userInput] = pickedUpCard;
    }
}

bool DetermineDealer()
{
    var random = new Random();
    if (random.NextDouble() >= 0.5) return true;
    else return false;
}

string CurrentPlayerString()
{
    if (isPlayerOneTurn) return "PLAYER ONE";
    else return "PLAYER TWO";
}


void FirstTurnChanceToPickupFromDiscardPile()
{
    while (userInput != 'd' && userInput != 'n' && breakSwitch == false)
    {
        userInput = ReadKey().KeyChar;

        switch (userInput)
        {
            case 'd':
                pickedUpCard = discardPile.Last();
                discardPile.Remove(discardPile.Last());
                WriteLine();
                WriteLine();
                WriteLine("Picked up " + pickedUpCard.ToString());

                WriteLine();
                WriteLine("Enter number 0-9 to select card from hand to discard.");
                WriteLine();

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
                            DiscardFromHand(isPlayerOneTurn, (int)Char.GetNumericValue(userInput)); // TODO: test to ensure correct player is selected
                            didNonDealerPickupAtFirstChance = true;
                            breakSwitch = true;
                            break;
                        default:
                            WriteLine();
                            WriteLine("Invalid input.\n");
                            break;
                    }
                }
                break;
            case 'n':
                WriteLine();
                WriteLine();
                WriteLine(CurrentPlayerString() + " has chosen to pass.");
                break;
            default:
                WriteLine();
                WriteLine("Invalid input.\n");
                break;
        }
    }

    breakSwitch = false;
    isPlayerOneTurn = !isPlayerOneTurn;
    userInput = ' ';

    // If non-dealer passed up first chance at discard pile, dealer is given chance to pickup the card
    if (!didNonDealerPickupAtFirstChance)
    {
        WriteLine();
        WriteLine("Non-dealer chose to pass - dealer now has chance to pick up card from discard pile.");
        WriteLine(CurrentPlayerString() + " - Press 'd' if you wish to pick up from the discard pile, or 'n' if you wish to pick up a new card from the deck.");
        WriteLine();

        // TODO: refactor into reusable method
        while (userInput != 'd' && userInput != 'n' && breakSwitch == false)
        {
            userInput = ReadKey().KeyChar;
            WriteLine();

            switch (userInput)
            {
                case 'd':
                    pickedUpCard = discardPile.Last();
                    discardPile.Remove(discardPile.Last());
                    WriteLine();
                    WriteLine();
                    WriteLine("Picked up " + pickedUpCard.ToString());

                    WriteLine();
                    WriteLine("Enter number 0-9 to select card from hand to discard.");
                    WriteLine();

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
                                breakSwitch = true;
                                break;
                            default:
                                WriteLine();
                                WriteLine("Invalid input.\n");
                                break;
                        }
                    }
                    break;
                case 'n':
                    WriteLine();
                    WriteLine();
                    WriteLine(CurrentPlayerString() + " has chosen to pass.");
                    break;
                default:
                    WriteLine();
                    WriteLine("Invalid input.\n");
                    break;
            }
        }

        isPlayerOneTurn = !isPlayerOneTurn;
    }

    handPlayerOne = HandMethods.DetectAndGroupByMelds(handPlayerOne);
    handPlayerTwo = HandMethods.DetectAndGroupByMelds(handPlayerTwo);
}

//foreach(var card in deck)
//{
//    Console.WriteLine(card.Suit + " " + card.Rank);
//    //Console.WriteLine((int)card.Suit + " " + (int)card.Rank);
//    Console.WriteLine(Environment.NewLine);
//}