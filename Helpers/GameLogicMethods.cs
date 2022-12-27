using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11242022_Gin_Rummy.Helpers
{
    public static class GameLogicMethods
    {
        public static bool DetermineDealer()
        {
            var random = new Random();
            if (random.NextDouble() >= 0.5) return true; // If assigned true, player one is dealer. Otherwise player two is dealer.
            else return false; 
        }

        public static string CurrentPlayerString(bool isPlayerOneTurn)
        {
            if (isPlayerOneTurn) return "PLAYER ONE";
            else return "PLAYER TWO";
        }
    }
}
