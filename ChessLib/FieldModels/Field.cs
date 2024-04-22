using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.PlayerModels;
using ChessLib.Figures;
using ChessLib.Enums.Players;

namespace ChessLib.FieldModels
{
    public class Field
    {
        public Cell[,] AllCells { get; set; }
        public List<Player> _players;

        private const int _fieldHeight = 8;
        private const int _fieldWidth = 8;

        private List<(int, int)> _secondPlayerPawnCords = new List<(int, int)>()
        {(1,0),(1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7)};
        private List<(int, int)> _secondPlayerRookCords = new List<(int, int)>()
        {(0,0),(0,7)};
        private List<(int, int)> _secondPlayerHorseCords = new List<(int, int)>()
        {(0,1),(0,6)};
        private List<(int, int)> _secondPlayerBishopCords = new List<(int, int)>()
        {(0,2),(0,5)};
        private (int, int) _secondPlayerQueenCord = (0, 3);
        private (int, int) _secondPlayerKingCord = (0, 4);

        private List<(int, int)> _firstPlayerPawnCords = new List<(int, int)>()
        {(6,0),(6,1),(6,2),(6,3),(6,4),(6,5),(6,6),(6,7)};
        private List<(int, int)> _firstPlayerRookCords = new List<(int, int)>()
        {(7,0),(7,7)};
        private List<(int, int)> _firstPlayerHorseCords = new List<(int, int)>()
        {(7,1),(7,6)};
        private List<(int, int)> _firstPlayerBishopCords = new List<(int, int)>()
        {(7,2),(7,5)};
        private (int, int) _firstPlayerQueenCord = (7, 3);
        private (int, int) _firstPlayerKingCord = (7, 4);

        public Field()
        {
            AllCells = new Cell[_fieldHeight, _fieldWidth];

            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    AllCells[i, j] = new Cell();
                }
            }
        }

        public Figure GetFigureToFill((int x, int y) cord)
        {
            if (_secondPlayerPawnCords.Contains(cord))
            {
                return new Pawn(_players.Find(x => x.Side == PlayerSide.Up).Color) 
            }
            


            return null;
        }

        public int GetAmountOfCells()
        {
            return AllCells.Length;
        }
    }
}
