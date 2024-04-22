using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Figures;
using ChessLib.Figures;

namespace ChessLib.Other
{
    public class Move
    {
        public List<(int, int)> OneMove { get; set; }
        public int[] HitHistIDs { get; set; } = new int[2];
        public ConvertPawn ConvertFigure { get; set; }
        public (int, int)? HitCellCordForBeatingOnThePass { get; set; }
        public Figure HitFigure { get; set; }
    }
}
