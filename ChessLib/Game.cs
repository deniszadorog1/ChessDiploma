using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.FieldModels;
using ChessLib.PlayerModels;
using ChessLib.Other;
using ChessLib.Figures;
using ChessLib.Enums.Players;

namespace ChessLib
{
    public class Game
    {
        public Field AllField { get; set; }
        public List<Player> Players { get; set; }

        private int _movesCounter = 0;

        private List<Move> _сheckForEqualMoves = new List<Move>();

        private int _movesWithoutHitCounter = 0;

        private int _toStartCheckForDraw = 12;
        private int _chosenEqualMovesToCheckForDraw = 4;
        private int _drawByMovesWithOutHitting = 50;

        private Player _steper; 

        public Game(Field allField, List<Player> players)
        {
            AllField = allField;
            Players = players;
        }
        public Game()
        {
            Players = new List<Player>();

            AddPlaeyrs();

            AllField = new Field(Players);
        }

        public void AddPlaeyrs()
        {
            Players.Add(new User("first", PlayerColor.Black, PlayerSide.Down));
            Players.Add(new User("second", PlayerColor.White, PlayerSide.Up));
            _steper = Players.Find(x => x.Color == PlayerColor.White);
        }
        public int GetFieldLegthParam()
        {
            return AllField.AllCells.Length;
        }
        public int GetFieldSizeParam()
        {
            return (int)Math.Sqrt(AllField.AllCells.Length);
        }

        public Figure InitCord((int x, int y) cord)
        {
            return AllField.AllCells[cord.x, cord.y].Figure;
        }
        public AllMoves GetMovesForFigure((int, int) figCord)
        {
            return AllField.GetMovesForFigure(figCord, _steper);
        }
        public void ReassignMove(Move move)
        {
            AllField.ReassignMove(move);
        }

        


    }
}
