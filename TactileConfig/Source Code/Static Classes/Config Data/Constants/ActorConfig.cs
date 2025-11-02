using System;
using System.Collections.Generic;
using System.Linq;

namespace Tactile.Constants
{
    public class Actor
    {
        public const int MAX_ACTOR_COUNT = short.MaxValue / 2; // Non-generic Ids must <= this value; generics start counting after this

        private static Dictionary<int, int> Prepromote_Levels = new Dictionary<int, int> { //<Actor ID, Prepromote Levels>
            {101, 10}, // Mazda
        }; 
        public static int prepomote_levels(int id)
        {
            int result = 0;
            if (Prepromote_Levels.ContainsKey(id))
                result = Prepromote_Levels[id];
            return result;
        }
    }
}
