using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;


namespace ChessLib.PlayerModels
{
    public class User : Player
    {
        public User(string name, PlayerColor playerColor, PlayerSide side) :
        base(name, playerColor, side)
        {
            Name = name;
            Color = playerColor;
            Side = side;
        }
        public User()
        {
            Name = "";
            Color = new PlayerColor();
            Side = new PlayerSide();
        }
    }
}
