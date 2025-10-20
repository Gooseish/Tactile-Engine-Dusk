using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TactileLibrary.Config
{
    class TerrainConfig
    {
        public readonly static Dictionary<int, int> TERRAIN_ALPHA_COST = new Dictionary<int, int>
        {
            { 10, 2 }, // Fort
            { 12, 2 }, // Forest
            { 13, 4 }, // Thicket
            { 18, 2 }, // Peak
            { 29, 2 }, // Pillar
        };
    }
}