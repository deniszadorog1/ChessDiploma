using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib;
using ChessLib.PlayerModels;
using ChessLib.Enums.Players;

namespace ChessDiploma.Models
{
    public static class Data
    {
        public static Game _game = new Game();

        public static void InitPlayerAndTimerInGame(Player player, Player enemy, int time)
        {
            UpdateGame();
            _game.Players = new List<Player>()
            {
                player,
                enemy
            };
            
            _game.InitTime(time * 60);
            _game.InitTimers();
            _game.InitSteper();

        }
        public static void UpdateGame()
        {
            _game = new Game();
        }
      

    }
}
