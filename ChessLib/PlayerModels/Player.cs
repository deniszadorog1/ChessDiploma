using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;

namespace ChessLib.PlayerModels
{
    public class Player
    {
        public string Name { get; set; }
        public PlayerColor Color { get; set; }
        public PlayerSide Side { get; set; }

        public Player(string name, PlayerColor color, PlayerSide side)
        {
            Name = name;
            Color = color;
            Side = side;
        }
        public Player()
        {
            Name = "";
            Color = new PlayerColor();
            Side = new PlayerSide();
        }
    }
}
