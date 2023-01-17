using _11242022_Gin_Rummy;
using static System.Console;

int playerOneScore = 0;
int playerTwoScore = 0;
int previousWinner = 0;

// ------------------------------------------------------------------------------

// Uncomment this code block to have two human players play against each other

//while ((playerOneScore < 100) && (playerTwoScore < 100))
//{
//    var gameInfo = GameRound.PlayRound(previousWinner);

//    playerOneScore += gameInfo[0];
//    playerTwoScore += gameInfo[1];
//    previousWinner = gameInfo[2];
//}

// ------------------------------------------------------------------------------

// Uncomment this code block to have a human play agains the simple AI agent

while ((playerOneScore < 100) && (playerTwoScore < 100))
{
    var gameInfo = GameRoundSimpleAgentVsHuman.PlayRound(previousWinner);

    playerOneScore += gameInfo[0];
    playerTwoScore += gameInfo[1];
    previousWinner = gameInfo[2];
}

// ------------------------------------------------------------------------------

// Uncomment this code block to have two simple AI agents play against each other

//for(int i = 0; i < 1000; i++)
//{
//    var gameInfo = GameRoundSimpleAgentVsSelf.PlayRound(previousWinner);

//    playerOneScore += gameInfo[0];
//    playerTwoScore += gameInfo[1];
//    previousWinner = gameInfo[2];
//}

WriteLine("\nFINAL SCORES:\n");
WriteLine("Player one score: " + playerOneScore);
WriteLine("Player two score: " + playerTwoScore);
WriteLine();