using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib;
using ChessLib.PlayerModels;
using ChessLib.Enums.Players;
using ChessLib.Other;

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
            _game.AllField = new ChessLib.FieldModels.Field(_game.Players);
            //_game.InitTimers();
            _game.InitTime(time * 60);
            _game.InitSteper();

        }
        public static void InitGameFieldToRepaly(Game game)
        {
            List<Move> moves = DbUsage.GetGameMoves(game);

            UpdateGame();

            _game.Players = new List<Player>()
            {
                game.Players[0],
                game.Players[1]
            };
            _game.AllField = new ChessLib.FieldModels.Field(_game.Players);
            //_game.InitTime(game.GetTime());
            _game.InitSteper();
            _game.InitMoveHistory(moves);

        }
        public static void UpdateGame()
        {
            _game = new Game();
        }
        public static void InitGameInDB()
        {
            UpdatePlayerAfterStepperGaveUp();
            DbUsage.InsertGame(_game);
            List<Move> movesHistory = _game.AllField.GetMoveHistory();
            for (int i = 0; i < movesHistory.Count; i++)
            {
                DbUsage.InsertMove(movesHistory[i]);
            }
        }
        public static void UpdatePlayerAfterStepperGaveUp()
        {
            _game.UpdetePlayersWhenSteperGaveUp();
            for(int i = 0; i < _game.Players.Count; i++)
            {
                if (_game.Players[i] is User)
                {
                    DbUsage.UpdateUsersResults((User)_game.Players[i]);
                }
            }
        }
        public static void InitGamesEndDate()
        {
            _game.EndTime = DateTime.Now;
        }

    }
}
