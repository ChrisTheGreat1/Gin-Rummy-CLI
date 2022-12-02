// See https://aka.ms/new-console-template for more information
using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Helpers;
using _11242022_Gin_Rummy.Models;
using System;
using System.Text;
using static System.Console;


var deck = DeckMethods.CreateDeck();
var discardPile = new List<Card>();
var pickedUpCard = new Card();
char userInput = ' ';

deck = DeckMethods.ShuffleDeck(deck);

var handPlayerOne = new List<Card>();
handPlayerOne.Capacity = 10;

var handPlayerTwo = new List<Card>();
handPlayerTwo.Capacity = 10;

// Deal hand
for(int i = 0; i < 10; i++)
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

// -------------------------------------------------------------------------------------------------------------------------------------------
// TODO: rng to determine who is dealer
// TODO: player who won previous game becomes dealer

// TODO: If the stock has run out and the next player does not want to take the discard,
// the game ends at that point. Everyone scores the value of the cards remaining in their hands.

// TODO: player who did not deal the cards starts the game, with the option to pick up the upturned card next to the stock
// deck. If the said card is of no interest, the player passes without discarding.
// The opponent may take that card and discard another, and if they are not interested, they pass without discarding.
// Then regular play with deck begins.

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


for (int i = 0; i < 10; i++)
{
    PrintHandsToConsole();

    WriteLine("Press 'd' if you wish to pick up from the discard pile, or 'n' if you wish to pick up a new card from the deck.");
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
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '1':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '2':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '3':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '4':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '5':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '6':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '7':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '8':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
                break;
            case '9':
                DiscardFromHand(discardPile, pickedUpCard, handPlayerOne, (int)Char.GetNumericValue(userInput));
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
}











//Console.ReadKey(true);


//foreach(var card in deck)
//{
//    Console.WriteLine(card.Suit + " " + card.Rank);
//    //Console.WriteLine((int)card.Suit + " " + (int)card.Rank);
//    Console.WriteLine(Environment.NewLine);
//}

//foreach (var card in handPlayerOne)
//{
//    Write(card.ToString());
//    Write(" ");
//}

WriteLine();

void PrintHandsToConsole()
{
    handPlayerOne = HandMethods.SortHandWithMeldGroupings(handPlayerOne);
    handPlayerTwo = HandMethods.SortHandWithMeldGroupings(handPlayerTwo);

    //WriteLine();
    //WriteLine(HandMethods.HandToString(handPlayerTwo));
    WriteLine();
    WriteLine("Discard pile: " + discardPile.Last().ToString());
    WriteLine();
    WriteLine(HandMethods.HandToString(handPlayerOne));
    WriteLine("0  1  2  3  4  5  6  7  8  9");
    WriteLine();
}

void DiscardFromHand(List<Card> discardPile, Card pickedUpCard, List<Card> handPlayerOne, int userInput)
{
        discardPile.Add(handPlayerOne[userInput]);
        WriteLine();
        WriteLine("Discarded " + discardPile.Last().ToString());
        handPlayerOne[userInput] = pickedUpCard;
}
//WriteLine();

//foreach (var card in handPlayerTwo)
//{
//    Write(card.ToString());
//    Write(" ");
//}