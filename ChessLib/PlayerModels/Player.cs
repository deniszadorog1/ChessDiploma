using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;
using ChessLib.Figures;
using ChessLib.Enums.Figures;
namespace ChessLib.PlayerModels
{
    public class Player
    {
        public string Login { get; set; }
        public PlayerColor Color { get; set; }
        public PlayerSide Side { get; set; }
        public List<(FigType name, int amount)> HitFigures { get; set; }

        public Player(string name, PlayerColor color, PlayerSide side, List<(FigType name, int amount)> hitFigures)
        {
            Login = name;
            Color = color;
            Side = side;
            HitFigures = hitFigures.Count != 0 ? hitFigures :
                new List<(FigType name, int amount)>()
            {
                (FigType.Pawn, 0),
                (FigType.Rook, 0),
                (FigType.Horse, 0),
                (FigType.Bishop, 0),
                (FigType.Queen, 0)
            };
        }
        public Player()
        {
            Login = "";
            Color = new PlayerColor();
            Side = new PlayerSide();
            HitFigures = new List<(FigType name, int amount)>()
            {
                (FigType.Pawn, 0),
                (FigType.Rook, 0),
                (FigType.Horse, 0),
                (FigType.Bishop, 0),
                (FigType.Queen, 0)
            };
        }
        public void ClearHitList()
        {
            HitFigures = new List<(FigType name, int amount)>()
            {
                (FigType.Pawn, 0),
                (FigType.Rook, 0),
                (FigType.Horse, 0),
                (FigType.Bishop, 0),
                (FigType.Queen, 0)
            };
        }

        public void UpdateHitFigures(Figure fig, int updater)
        {
            if (fig is Pawn)
            {
                int index = HitFigures.FindIndex(x => x.name == FigType.Pawn);
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = (FigType.Pawn, newAmount);
            }
            else if (fig is Rook)
            {
                int index = HitFigures.FindIndex(x => x.name == FigType.Rook);
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = (FigType.Rook, newAmount);
            }
            else if (fig is Horse)
            {
                int index = HitFigures.FindIndex(x => x.name == FigType.Horse);
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = (FigType.Horse, newAmount);
            }
            else if (fig is Bishop)
            {
                int index = HitFigures.FindIndex(x => x.name == FigType.Bishop);
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = (FigType.Bishop, newAmount);
            }
            else if (fig is Queen)
            {
                int index = HitFigures.FindIndex(x => x.name == FigType.Queen);
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = (FigType.Queen, newAmount);
            }
        }
    }
}
