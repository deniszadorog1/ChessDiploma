
using ChessLib.Enums.Field;
using ChessLib.Figures;

namespace ChessLib.FieldModels
{
    public class Cell
    {
        public CellColor Color { get; set; }
        public Figure Figure { get; set; }

        public Cell(CellColor color, Figure figure)
        {
            Color = color;
            Figure = figure;
        }
        public Cell()
        {
            Color = new CellColor();
            Figure = null;
        }
    }
}
