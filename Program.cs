// See https://aka.ms/new-console-template for more information
using _11242022_Gin_Rummy.Enums;
using _11242022_Gin_Rummy.Helpers;
using _11242022_Gin_Rummy.Models;
using System;
using System.Text;
using static System.Console;


var deck = DeckMethods.CreateDeck();
var discardPile = new List<Card>();

deck = DeckMethods.ShuffleDeck(deck);

var handPlayerOne = new List<Card>();
handPlayerOne.Capacity = 10;

var handPlayerTwo = new List<Card>();
handPlayerTwo.Capacity = 10;

// Deal hand
for(int i = 0; i < 10; i++)
{
    handPlayerOne.Add(deck.First());
    deck.Remove(deck.First());

    handPlayerTwo.Add(deck.First());
    deck.Remove(deck.First());
}

discardPile.Add(deck.First());
deck.Remove(deck.First());

handPlayerOne = HandMethods.SortHand(handPlayerOne);

HandMethods.DetectMelds(handPlayerOne);











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

//WriteLine();
//WriteLine();

//foreach (var card in handPlayerTwo)
//{
//    Write(card.ToString());
//    Write(" ");
//}