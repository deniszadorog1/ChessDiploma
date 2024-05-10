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
using ChessLib.Figures.Interfaces;

namespace ChessLib.Figures
{
    public class King : Figure, IFirstMove
    {
        public bool IsFirstMoveMaken { get; set; }
        public List<(int, int)> DirectionsToStep = new List<(int, int)>()
        {
            (1,0),
            (-1, 0),
            (0,1),
            (0,-1),
            (1,1),
            (-1,-1),
            (-1,1),
            (1,-1)
        };
        private readonly double[,] posTableDownPlayerMidGame = new double[8, 8] {
            {-0.3, -0.4, -0.4, -0.5, -0.5, -0.4, -0.4, -0.3},
            {-0.3, -0.4, -0.4, -0.5, -0.5, -0.5, -0.4, -0.3},
            {-0.3, -0.4, -0.4, -0.5, -0.5, -0.4, -0.4, -0.3},
            {-0.2, -0.4, -0.4, -0.5, -0.5, -0.4, -0.4, -0.3},
            {-0.2, -0.3, -0.3, -0.4, -0.4, -0.3, -0.3, -0.2},
            {-0.1, -0.2, -0.2, -0.2, -0.2, -0.2, -0.2, -0.1},
            {0.2, 0.2, 0.0, 0.0, 0.0, 0.0, 0.2, 0.2},
            {0.2, 0.3, 0.1, 0.0, 0.0, 0.1, 0.3, 0.2}
        };
        public King(PlayerColor figColor, bool ifMakenOneMove, 
            int figureId, (int,int) figCord, PlayerSide ownerSide) : 
            base(figColor, figureId, figCord, ownerSide)
        {
            IsFirstMoveMaken = ifMakenOneMove;
        }
        public King()
        {
            IsFirstMoveMaken = false;
        }
        public override double GetScoreForFigure()
        {
            return _scoreForKing;
        }
        public override AllMoves GetHitMoves(Field field, Player player, (int, int) figCord)
        {
            AllMoves res = new AllMoves();
            for (int i = 0; i < DirectionsToStep.Count; i++)
            {
                (int, int) check = (figCord.Item1 + DirectionsToStep[i].Item1, figCord.Item2 + DirectionsToStep[i].Item2);

                if (!field.IfChipIsOutOfRange(check) &&
                    field.IfChipIsEnemys(player, check))
                {
                    res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, check },
                    new int[] { FigureID, field.AllCells[check.Item1, check.Item2].Figure.FigureID }));
                }
            }
            return res;
        }
        public override Figure GetCopy()
        {
            return new King(FigureColor, IsFirstMoveMaken, FigureID, FigureCord, OwnerSide);
        }
        public override double GetScoreForFigPositionOnBoard((int, int) cord)
        {
            return OwnerSide == PlayerSide.Up ? posTableDownPlayerMidGame[cord.Item1, cord.Item2] :
            posTableDownPlayerMidGame[_maxCordOnBoard - cord.Item1, _maxCordOnBoard - cord.Item2];
        }
        public override AllMoves GetMoves(Player player,
        (int, int) cord, Field field, KingRays kingRays)
        {
            AllMoves moves = new AllMoves();

            for (int i = 0; i < DirectionsToStep.Count; i++)
            {
                (int, int) cordToStepOn = (cord.Item1 + DirectionsToStep[i].Item1,
                    cord.Item2 + DirectionsToStep[i].Item2);

                if (!field.IfChipIsOutOfRange(cordToStepOn) && !field.IfChipIsPlayers(player, cordToStepOn) &&
                   !field.IfKingWillBeHitAfterMove(field,
                   new Move(new List<(int, int)>() { cord, cordToStepOn }), player))
                {
                    if (field.AllCells[cordToStepOn.Item1, cordToStepOn.Item2].Figure != null)
                    {
                        moves.PossibleMoves.Add(new Move(new List<(int, int)>() { cord, cordToStepOn },
                             new int[] { field.AllCells[cord.Item1, cord.Item2].Figure.FigureID,
                             field.AllCells[cordToStepOn.Item1, cordToStepOn.Item2].Figure.FigureID}));
                    }
                    else
                    {
                        moves.PossibleMoves.Add(new Move(new List<(int, int)>() { cord, cordToStepOn }));

                    }
                }
            }
            if (!IsFirstMoveMaken)
            {
                moves.PossibleMoves.AddRange(field.
                    GetMovesForCastling(field, player, cord).PossibleMoves);
            }
            return moves;
        }
        
       
    }
}
