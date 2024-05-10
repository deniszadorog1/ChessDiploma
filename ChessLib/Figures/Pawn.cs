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
    public class Pawn : Figure, IFirstMove
    {
        public bool IsFirstMoveMaken { get; set; }

        private readonly (int, int) _divisionToMoveForFirstPlayer = (-1, 0);
        public List<(int, int)> _divisionToHitForFirstPlayer = new List<(int, int)>() { (-1, -1), (-1, 1) };

        private readonly (int, int) _divisionToMoveForSecondPlayer = (1, 0);
        public List<(int, int)> _divisionToHitForSecondPlayer = new List<(int, int)>() { (1, -1), (1, 1) };

        private readonly double[,] posTableDownPlayer = new double[8, 8] {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5},
            {0.1, 0.1, 0.2, 0.25, 0.25, 0.2, 0.1, 0.1},
            {0.05, 0.05, 0.1, 0.25, 0.25, 0.1, 0.05, 0.05},
            {0, 0, 0, 0.2, 0.2, 0, 0, 0},
            {0.05, -0.05, -0.1, 0, 0, -0.1, -0.05, 0.05},
            {0.05, 0.1, 0.1, -0.2, -0.2, 0.1, 0.1, 0.05},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        private const int _longMoveDistance = 2;
        private const int _shortMove = 1;

        public Pawn(PlayerColor figColor,
            bool ifMakenOneMove, int figureId, (int,int) figCord, PlayerSide ownerSide) : 
            base(figColor, figureId, figCord, ownerSide)
        {
            FigureColor = figColor;
            IsFirstMoveMaken = ifMakenOneMove;
            FigureID = figureId;
            FigureCord = figCord;
            OwnerSide = ownerSide;
        }

        public override double GetScoreForFigPositionOnBoard((int, int) cord)
        {
            return OwnerSide == PlayerSide.Up ? posTableDownPlayer[cord.Item1, cord.Item2] :
            posTableDownPlayer[_maxCordOnBoard - cord.Item1, _maxCordOnBoard - cord.Item2];
        }
        public override Figure GetCopy()
        {
            return new Pawn(FigureColor, IsFirstMoveMaken, FigureID, FigureCord, OwnerSide);
        }
        public override double GetScoreForFigure()
        {
            return _scoreForPawn;
        }
        public override AllMoves GetHitMoves(Field field, Player player, (int, int) figCord)
        {
            AllMoves res = new AllMoves();

            List<(int, int)> hitSides = player.Side == PlayerSide.Down ? _divisionToHitForFirstPlayer : _divisionToHitForSecondPlayer;

            for (int i = 0; i < hitSides.Count &&
                !field.IfChipIsOutOfRange((figCord.Item1 + hitSides[i].Item1, figCord.Item2 + hitSides[i].Item2)); i++)
            {
                (int, int) check = (figCord.Item1 + hitSides[i].Item1, figCord.Item2 + hitSides[i].Item2);

                if (field.AllCells[check.Item1, check.Item2].Figure != null)
                {
                    if (field.AllCells[check.Item1, check.Item2].Figure.FigureColor != player.Color)
                    {
                        if (check.Item1 == _maxCordOnBoard || check.Item1 == 0)
                        {
                            for (int j = (int)ConvertPawn.Queen; j <= (int)ConvertPawn.Bishop; j++)
                            {
                                res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, check },
                                    (ConvertPawn)j,
                                    new int[] { FigureID, field.AllCells[check.Item1, check.Item2].Figure.FigureID },
                                    field.AllCells[check.Item1, check.Item2].Figure.GetCopy()));
                            }
                        }
                        else
                        {

                            res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, check },
                                new int[] { FigureID, field.AllCells[check.Item1, check.Item2].Figure.FigureID }));
                        }
                    }
                    break;
                }

            }
            return res;
        }
        public override AllMoves GetMoves(Player player, (int, int) pawnCord, Field field, KingRays kingRays)
        {
            if (kingRays.FigsThatHitsKing.Count > 1 ||
                (kingRays.FigsThatProtectsKing.Contains(pawnCord) && kingRays.FigsThatHitsKing.Count > 0))
            {
                return new AllMoves();
            }

            List<(int, int)> cordsToStepOn = GetCordsToStepOn(pawnCord, field, player);

            AllMoves res = (kingRays.FigsThatProtectsKing.Contains(pawnCord) &&
                kingRays.FigsThatHitsKing.Count == 0) ? GetMovesForPawnIfPawnProtKing(cordsToStepOn, player, kingRays, pawnCord, field) :

                (!kingRays.FigsThatProtectsKing.Contains(pawnCord) &&
                kingRays.FigsThatHitsKing.Count == 1) ? GetMovesToProtectKing(player, cordsToStepOn, kingRays, pawnCord) :

                GetAllMovesForPawn(cordsToStepOn, pawnCord, player);

            for (int i = 0; i < res.PossibleMoves.Count; i++)
            {
                (int, int) lastStepCord = res.PossibleMoves[i].OneMove.Last();

                if (field.AllCells[lastStepCord.Item1, lastStepCord.Item2].Figure != null)
                {
                    res.PossibleMoves[i].HitHistIDs = new int[]
                    {
                        FigureID,
                        field.AllCells[lastStepCord.Item1,lastStepCord.Item2].Figure.FigureID
                    };
                    res.PossibleMoves[i].HitFigure = field.AllCells[lastStepCord.Item1,
                        lastStepCord.Item2].Figure.GetCopy();
                }
            }
            return res;
        }
        public AllMoves GetAllMovesForPawn(List<(int, int)> cordsToStepOn, (int, int) pawnCord, Player player)
        {
            AllMoves res = new AllMoves();
            cordsToStepOn.ForEach(x => res.PossibleMoves.AddRange(GetMovesForPawnIfItWentToThatEndOfBoard(player, new List<(int, int)>()
                    {pawnCord, x }).PossibleMoves.ToList()));

            return res;
        }
        public AllMoves GetMovesToProtectKing(Player player, List<(int, int)> cordToStepOn,
            KingRays kingRays, (int, int) pawnCord)
        {
            AllMoves res = new AllMoves();

            for (int i = 0; i < kingRays.AllRays.Count; i++)
            {
                if (kingRays.FigsThatHitsKing.Contains(kingRays.AllRays[i].Last()))
                {
                    for (int j = 0; j < cordToStepOn.Count; j++)
                    {
                        if (kingRays.AllRays[i].Contains(cordToStepOn[j]))
                        {
                            res.PossibleMoves =
                               res.PossibleMoves.Concat(GetMovesForPawnIfItWentToThatEndOfBoard(player, new List<(int, int)>()
                               {pawnCord, cordToStepOn[j] }).PossibleMoves).ToList();
                        }
                    }
                }
            }
            return res;
        }
        public AllMoves GetMovesForPawnIfPawnProtKing(List<(int, int)> cordsToStepOn,
        Player player, KingRays rays, (int, int) pawnCord, Field field)
        {
            return rays.AllRays.Any(x => x.Contains(pawnCord)) ? GetMovesThatCanBemMakenWhileProtectingKingVarTwo(pawnCord,
                        field.GetDirectionBetweenTwoFig(rays.KingCord, pawnCord), cordsToStepOn, player) : new AllMoves();
        }
        public AllMoves GetMovesThatCanBemMakenWhileProtectingKingVarTwo((int, int) pawnCord,
           (int, int) direction, List<(int, int)> cordsToStepOn, Player player)
        {
            AllMoves res = new AllMoves();
            (int, int) temp = pawnCord;
            for (int i = 0; i < cordsToStepOn.Count; i++)
            {
                if (cordsToStepOn[i] == (temp.Item1 + direction.Item1, temp.Item2 + direction.Item2))
                {
                    res.PossibleMoves.AddRange((GetMovesForPawnIfItWentToThatEndOfBoard(player, new List<(int, int)>()
                    {pawnCord, (temp.Item1 + direction.Item1, temp.Item2 + direction.Item2) }).PossibleMoves));

                    temp = (temp.Item1 + direction.Item1, temp.Item2 + direction.Item2);
                }
            }
            return res;
        }
        public AllMoves GetMovesForPawnIfItWentToThatEndOfBoard(Player player, List<(int, int)> move)
        {
            AllMoves res = new AllMoves();

            if ((player.Side == PlayerSide.Down && move.Last().Item1 == 0) ||
                (player.Side == PlayerSide.Up && move.Last().Item1 == _maxCordOnBoard))
            {
                for (int i = (int)ConvertPawn.Queen; i <= (int)ConvertPawn.Bishop; i++)
                {
                    res.PossibleMoves.Add(new Move(new List<(int, int)>(move), (ConvertPawn)i));
                }
            }
            else
            {
                res.PossibleMoves.Add(new Move(move));
            }

            return res;
        }
        public List<(int, int)> GetCordsToStepOn((int, int) pawnCord, Field field, Player player)
        {
            //List<(int, int)> res = new List<(int, int)>();

            (int, int) usualStepDivision = (player.Side == PlayerSide.Down) ? _divisionToMoveForFirstPlayer : _divisionToMoveForSecondPlayer;
            int distanse = IsFirstMoveMaken ? _shortMove : _longMoveDistance;
            List<(int, int)> getHitDivision = (player.Side == PlayerSide.Down) ? _divisionToHitForFirstPlayer : _divisionToHitForSecondPlayer;

            //Getting usual moves

            return GetUsualMoves(field, usualStepDivision, distanse, pawnCord).Concat(GetHitMovesForPawn(field, getHitDivision, pawnCord, player)).ToList();
        }
        public List<(int, int)> GetUsualMoves(Field field, (int, int) division,
        int distance, (int, int) figCord)
        {
            List<(int, int)> res = new List<(int, int)>();
            (int, int) temp = (figCord.Item1 + division.Item1, figCord.Item2 + division.Item2);

            if (field.AllCells[temp.Item1, temp.Item2].Figure == null)
            {
                res.Add(temp);

                if (!field.IfChipIsOutOfRange((temp.Item1 + division.Item1, temp.Item2 + division.Item2)) &&
                    distance == _longMoveDistance && field.AllCells[(temp.Item1 + division.Item1), (temp.Item2 + division.Item2)].Figure == null)
                {
                    res.Add((temp.Item1 + division.Item1, temp.Item2 + division.Item2));
                }
            }
            return res;

        }
        public List<(int, int)> GetHitMovesForPawn(Field field, List<(int, int)> division, (int, int) figCord, Player player)
        {
            return division
                .Select(div => (figCord.Item1 + div.Item1, figCord.Item2 + div.Item2))
                .Where(temp => !field.IfChipIsOutOfRange(temp) && field.IfChipIsEnemys(player, temp))
                .ToList();

        }
    }
}
