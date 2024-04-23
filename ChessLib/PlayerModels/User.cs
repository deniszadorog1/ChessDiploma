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
        public User(string name, PlayerColor playerColor, PlayerSide side, List<(string name, int amount)> hitFigures) :
        base(name, playerColor, side, hitFigures)
        {
            Name = name;
            Color = playerColor;
            Side = side;
        }
        public User() : base()
        {
           
        }
    }
}
