using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.FieldModels;
using ChessLib.Players;
using ChessLib.Other;
using ChessLib.Figures;

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

        public Game(Field allField, List<Player> players)
        {
            AllField = allField;
            Players = players;
        }
        public Game()
        {
            AllField = new Field();
            Players = new List<Player>();
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


            return new Figure();
        }




    }
}
