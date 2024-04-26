using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;
using ChessLib.Figures;

namespace ChessLib.PlayerModels
{
    public class Player
    {
        public string Login { get; set; }
        public PlayerColor Color { get; set; }
        public PlayerSide Side { get; set; }
        public List<(string name, int amount)> HitFigures { get; set; }

        public Player(string name, PlayerColor color, PlayerSide side, List<(string name, int amount)> hitFigures)
        {
            Login = name;
            Color = color;
            Side = side;
            HitFigures = hitFigures.Count != 0 ? hitFigures :
                new List<(string name, int amount)>()
            {
                ("pawn", 0),
                ("rook", 0),
                ("horse", 0),
                ("bishop", 0),
                ("queen", 0),
            };
        }
        public Player()
        {
            Login = "";
            Color = new PlayerColor();
            Side = new PlayerSide();
            HitFigures = new List<(string name, int amount)>()
            {
                ("pawn", 0),
                ("rook", 0),
                ("horse", 0),
                ("bishop", 0),
                ("queen", 0),
            };
        }



        public void UpdateHitFigures(Figure fig, int updater)
        {
            if (fig is Pawn)
            {
                int index = HitFigures.FindIndex(x => x.name == "pawn");
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = ("pawn", newAmount);
            }
            else if (fig is Rook)
            {
                int index = HitFigures.FindIndex(x => x.name == "rook");
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = ("rook", newAmount);
            }
            else if (fig is Horse)
            {
                int index = HitFigures.FindIndex(x => x.name == "horse");
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = ("horse", newAmount);
            }
            else if (fig is Bishop)
            {
                int index = HitFigures.FindIndex(x => x.name == "bishop");
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = ("bishop", newAmount);
            }
            else if (fig is Queen)
            {
                int index = HitFigures.FindIndex(x => x.name == "queen");
                int newAmount = HitFigures[index].amount;
                newAmount += updater;
                HitFigures[index] = ("queen", newAmount);
            }
        }
    }
}
