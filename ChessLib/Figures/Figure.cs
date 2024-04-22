using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;

namespace ChessLib.Figures
{
    public class Figure
    {
        public PlayerColor FigureColor { get; set; }
        public int FigureID { get; set; }
        public (int,int) FigureCord { get; set; }

        public Figure()
        {
            FigureColor = new PlayerColor();
            FigureID = -1;
            FigureCord = (-1, -1);
        }
        public Figure(PlayerColor figureColor,
            int figureId, (int,int) figureCord)
        {
            FigureColor = figureColor;
            FigureID = figureId;
            FigureCord = figureCord;
        }
    }
}
