using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Other
{
    public class KingRays
    {

        public List<List<(int, int)>> AllRays { get; set; }
        public List<(int, int)> FigsThatHitsKing { get; set; }
        public List<(int, int)> FigsThatProtectsKing { get; set; }
        public (int, int) KingCord { get; set; }
   
    }
}
