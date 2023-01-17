using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11242022_Gin_Rummy.Helpers
{
    public static class GameLogicMethods
    {
        /// <summary>
        /// Randomly determine the dealer. If true is returned, player one is dealer. If false is returned, player two is dealer.
        /// </summary>
        /// <returns></returns>
        public static bool DetermineDealer()
        {
            var random = new Random();
            if (random.NextDouble() >= 0.5) return true; // If assigned true, player one is dealer. Otherwise player two is dealer.
            else return false; 
        }

        /// <summary>
        /// Returns string denoting whose turn it is ("PLAYER ONE" or "PLAYER TWO").
        /// </summary>
        /// <param name="isPlayerOneTurn"></param>
        /// <returns></returns>
        public static string CurrentPlayerString(bool isPlayerOneTurn)
        {
            if (isPlayerOneTurn) return "PLAYER ONE";
            else return "PLAYER TWO";
        }
    }
}
