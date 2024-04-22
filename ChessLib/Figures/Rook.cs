using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;
using ChessLib.Other;
using ChessLib.PlayerModels;
using ChessLib.FieldModels;
using ChessLib.Enums.Figures;

namespace ChessLib.Figures
{
    public class Rook : Figure
    {
        public bool IfFirstMoveMaken { get; set; }

        private readonly double[,] posTableDownPlayer = new double[8, 8] {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0.05, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.05},
            {-0.05, 0, 0, 0, 0, 0, 0, -0.05},
            {-0.05, 0, 0, 0, 0, 0, 0, -0.05},
            {-0.05, 0, 0, 0, 0, 0, 0, -0.05},
            {-0.05, 0, 0, 0, 0, 0, 0, -0.05},
            {-0.05, 0, 0, 0, 0, 0, 0, -0.05},
            {0, 0, 0, 0.05, 0.05, 0, 0, 0}
        };

        public Rook(PlayerColor figColor, 
            bool ifMoveIsMaken, int figureId, (int,int) figCord, PlayerSide ownerSide) :
            base(figColor, figureId, figCord, ownerSide)
        {
            IfFirstMoveMaken = ifMoveIsMaken;
        }
        public Rook()
        {
            IfFirstMoveMaken = false;
        }
        public override double GetScoreForFigure()
        {
            return _scoreForRook;
        }
        public override Figure GetCopy()
        {
            return new Rook(FigureColor, IfFirstMoveMaken, FigureID, FigureCord, OwnerSide);
        }
        public override AllMoves GetHitMoves(Field field, Player player, (int, int) figCord)
        {
            AllMoves res = new AllMoves();

            for (int i = 0; i < _forwardDirection.Count; i++)
            {
                (int, int) check = figCord;
                do
                {
                    check = (check.Item1 + _forwardDirection[i].Item1, check.Item2 + _forwardDirection[i].Item2);
                    if (field.IfChipIsOutOfRange(check))
                    {
                        break;
                    }
                    else if (field.AllCells[check.Item1, check.Item2].Figure != null)
                    {
                        if (field.AllCells[check.Item1, check.Item2].Figure.FigureColor != player.Color)
                        {
                            res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, check },
                                new int[] { FigureID, field.AllCells[check.Item1, check.Item2].Figure.FigureID }));
                        }
                        break;
                    }
                } while (true);
            }
            return res;
        }
        public override double GetScoreForFigPositionOnBoard((int, int) cord)
        {
            return OwnerSide == PlayerSide.Up ? posTableDownPlayer[cord.Item1, cord.Item2] :
            posTableDownPlayer[_maxCordOnBoard - cord.Item1, _maxCordOnBoard - cord.Item2];
        }

    }
}
