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
using _11242022_Gin_Rummy;

// TODO: hotkey for displaying hand history
// TODO: counter for number of rounds played in current game

int playerOneScore = 0;
int playerTwoScore = 0;
int previousWinner = 0;

//while ((playerOneScore < 100) && (playerTwoScore < 100))
//{
//    var gameInfo = GameRound.PlayRound(previousWinner);

//    playerOneScore += gameInfo[0];
//    playerTwoScore += gameInfo[1];
//    previousWinner = gameInfo[2];
//}

//while ((playerOneScore < 100) && (playerTwoScore < 100))
//{
//    var gameInfo = GameRoundSimpleAgentVsHuman.PlayRound(previousWinner);

//    playerOneScore += gameInfo[0];
//    playerTwoScore += gameInfo[1];
//    previousWinner = gameInfo[2];
//}

for(int i = 0; i < 100; i++)
{
    var gameInfo = GameRoundSimpleAgentVsSelf.PlayRound(previousWinner);

    playerOneScore += gameInfo[0];
    playerTwoScore += gameInfo[1];
    previousWinner = gameInfo[2];
}

WriteLine("\nFINAL SCORES:\n");
WriteLine("Player one score: " + playerOneScore);
WriteLine("Player two score: " + playerTwoScore);
WriteLine();