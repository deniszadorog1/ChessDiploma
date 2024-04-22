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
    public class Bishop: Figure
    {
        private readonly double[,] posTableDownPlayer = new double[8, 8] {
            { -0.2, -0.1, -0.1, -0.1, -0.1, -0.1, -0.1, -0.2 },
            {-0.1, 0, 0, 0, 0, 0, 0, -0.1},
            {-0.1, 0, 0.05, 0.1, 0.1, 0.05, 0, -0.1},
            {-0.1, 0.05, 0.05, 0.1, 0.1, 0.05, 0.05, -0.1},
            {-0.1, 0.0, 0.1, 0.1, 0.1, 0.1, 0.0, -0.1},
            {-0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, -0.1},
            {-0.1, 0.05, 0, 0, 0, 0, 0.05, -0.1},
            {-0.2, -0.1, -0.1, -0.1, -0.1, -0.1, -0.1, -0.2}
        };
        public Bishop(PlayerColor figColor, int figureId, (int,int) figCord, PlayerSide ownerSide) : 
            base(figColor, figureId, figCord, ownerSide)
        {

        }
        public Bishop()
        {
            FigureColor = new PlayerColor();
        }

        public override AllMoves GetHitMoves(Field field, Player player, (int, int) figCord)
        {
            AllMoves res = new AllMoves();

            for (int i = 0; i < _crossDirections.Count; i++)
            {
                (int, int) check = figCord;
                do
                {
                    check = (check.Item1 + _crossDirections[i].Item1, check.Item2 + _crossDirections[i].Item2);
                    if (field.IfChipIsOutOfRange(check))
                    {
                        break;
                    }
                    else if (field.AllCells[check.Item1, check.Item2].Figure != null)
                    {
                        if (field.AllCells[check.Item1, check.Item2].Figure.FigureColor != player.Color)
                        {
                            res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, check },
                                new int[] { FigureID, field.AllCells[check.Item1, check.Item2].Figure.FigureID },
                                field.AllCells[check.Item1, check.Item2].Figure.GetCopy()));
                        }
                        break;
                    }
                } while (true);
            }
            return res;
        }
        public override double GetScoreForFigure()
        {
            return _scoreForBishop;
        }
        public override Figure GetCopy()
        {
            return new Bishop(FigureColor, FigureID, FigureCord, OwnerSide);
        }
        public override double GetScoreForFigPositionOnBoard((int, int) cord)
        {
            return OwnerSide == PlayerSide.Up ? posTableDownPlayer[cord.Item1, cord.Item2] :
                posTableDownPlayer[_maxCordOnBoard - cord.Item1, _maxCordOnBoard - cord.Item2];

        }
    }
}
