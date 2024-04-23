using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;
using ChessLib.Other;
using ChessLib.PlayerModels;
using ChessLib.FieldModels;

namespace ChessLib.Figures
{
    public abstract class Figure
    {
        public PlayerColor FigureColor { get; set; }
        public int FigureID { get; set; }
        public (int, int) FigureCord { get; set; }
        public PlayerSide OwnerSide { get; set; }

        public Figure()
        {
            FigureColor = new PlayerColor();
            FigureID = -1;
            FigureCord = (-1, -1);
            OwnerSide = new PlayerSide();
        }
        public Figure(PlayerColor chipColor, int figureId)
        {
            FigureColor = chipColor;
            FigureID = figureId;
        }
        public Figure(PlayerColor figureColor,
            int figureId, (int, int) figureCord, PlayerSide ownerSide)
        {
            FigureColor = figureColor;
            FigureID = figureId;
            FigureCord = figureCord;
            OwnerSide = ownerSide;
        }
        protected readonly List<(int, int)> _forwardDirection = new List<(int, int)>()
        {
            (-1,0),
            (0,1),
            (1,0),
            (0,-1)
        };
        protected readonly List<(int, int)> _crossDirections = new List<(int, int)>()
        {
            (-1,-1),
            (-1, 1),
            (1,1),
            (1,-1)
        };
        public abstract AllMoves GetHitMoves(Field field,
            Player player, (int, int) figCord);
        public abstract Figure GetCopy();
        public abstract double GetScoreForFigure();
        public abstract double GetScoreForFigPositionOnBoard((int, int) cord);

        protected int _maxCordOnBoard = 7;

        protected int _scoreForPawn = 1;
        protected int _scoreForRook = 5;
        protected int _scoreForHorse = 3;
        protected int _scoreForBishop = 3;
        protected int _scoreForKing = 10000;
        protected int _scoreForQueen = 9;

        public virtual AllMoves GetMoves(Player player,
           (int, int) cord, Field field, KingRays kingRays)
        {
            AllMoves moves = new AllMoves();
            // getting all moves
            if (!kingRays.FigsThatProtectsKing.Contains(cord) &&
                kingRays.FigsThatHitsKing.Count == 0)
            {
                List<(int, int)> toCheckDirections = GetDirectionsToCheck(field, cord);
                for (int i = 0; i < toCheckDirections.Count; i++)
                {
                    moves.PossibleMoves.AddRange(GetMovesToMakeIfITsNotContainsInRays(field, cord, toCheckDirections[i], player).PossibleMoves);
                }
                return moves;
            }
            //get directions to check
            List<(int, int)> divisions = GetDirections(field, cord, kingRays, player, kingRays.KingCord);
            if (kingRays.FigsThatHitsKing.Count > 0 &&
                !kingRays.FigsThatProtectsKing.Contains(cord))
            {
                for (int i = 0; i < divisions.Count; i++)
                {
                    moves.PossibleMoves.AddRange(GetMovesInGivenWayIfKingIsnderAtack
                    (field, cord, divisions[i], player, kingRays.KingCord).PossibleMoves);
                }
            }
            else if (kingRays.FigsThatProtectsKing.Count > 0 &&
                kingRays.FigsThatProtectsKing.Contains(cord))
            {
                List<(int, int)> rayThatContaiinsFig = GetListThatContainsCord(kingRays.AllRays, cord);
                if (rayThatContaiinsFig.Count == 0)
                {
                    return moves;
                }
                for (int i = 0; i < divisions.Count; i++)
                {
                    moves.PossibleMoves.AddRange(GetMovesForFigureIfItProtectsKing(field, cord,
                        divisions[i], rayThatContaiinsFig).PossibleMoves);
                }
            }
            else
            {
                for (int i = 0; i < divisions.Count; i++)
                {
                    moves.PossibleMoves.AddRange(GetMovesInGivenDirection
                    (field, cord, divisions[i], player, kingRays).PossibleMoves);
                }
            }

            return moves;
        }
        public AllMoves GetMovesInGivenDirection(Field field, (int, int) figCord,
        (int, int) direction, Player player, KingRays kingRays)
        {
            AllMoves res = new AllMoves();
            (int, int) tempCord = figCord;

            do
            {
                tempCord = (tempCord.Item1 + direction.Item1, tempCord.Item2 + direction.Item2);

                if (field.IfChipIsOutOfRange(tempCord))
                {
                    return res;
                }
                else if (field.AllCells[tempCord.Item1, tempCord.Item2].Figure != null)
                {
                    if (field.AllCells[tempCord.Item1, tempCord.Item2].
                        Figure.FigureColor != player.Color &&
                        !IfKingStillCanBeHit(tempCord, kingRays, field, player))
                    {
                        res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, tempCord }));
                    }
                    return res;
                }
                else if (!IfKingStillCanBeHit(tempCord, kingRays, field, player))
                {
                    res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, tempCord }));
                }
            } while (true);
        }
        public bool IfKingStillCanBeHit((int, int) tempCord, KingRays kingRays, Field field, Player player)
        {
            return kingRays.AllRays.Any(x => field.IfChipIsEnemys(player, x.Last()) &&
            !x.Contains(tempCord));
        }
        public AllMoves GetMovesForFigureIfItProtectsKing(Field field, (int, int) figCord,
        (int, int) direction, List<(int, int)> rayThatContainsFigCord)
        {
            AllMoves res = new AllMoves();
            (int, int) tempCord = figCord;

            do
            {
                tempCord = (tempCord.Item1 + direction.Item1, tempCord.Item2 + direction.Item2);

                if (field.IfChipIsOutOfRange(tempCord) || !rayThatContainsFigCord.Contains(tempCord))
                {
                    return res;
                }
                else
                {
                    res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, tempCord }));
                }
            } while (true);
        }
        public List<(int, int)> GetListThatContainsCord(List<List<(int, int)>> allRays, (int, int) cord)
        {
            return allRays.Any(x => x.Contains(cord)) ?
                allRays.First(x => x.Contains(cord)) : new List<(int, int)>();

        }
        public AllMoves GetMovesInGivenWayIfKingIsnderAtack(Field field, (int, int) figCord,
        (int, int) direction, Player player, (int, int) kingCord)
        {
            AllMoves res = new AllMoves();
            (int, int) tempCord = figCord;

            do
            {
                tempCord = (tempCord.Item1 + direction.Item1, tempCord.Item2 + direction.Item2);

                if (field.IfChipIsOutOfRange(tempCord))
                {
                    return res;
                }
                else if (!IfKingWillBeHitAfterMakenNotKingMove(field, player,
                    new Move(new List<(int, int)>() { figCord, tempCord }), kingCord))
                {
                    res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, tempCord }));
                    return res;
                }
            } while (true);
        }
        public bool IfKingWillBeHitAfterMakenNotKingMove(Field field, Player player, Move move, (int, int) kingCord)
        {
            if (move.OneMove.Last().Item1 < 0 || move.OneMove.Last().Item2 < 0)
            {
                return true;
            }

            Field tempField = new Field(field);
            tempField.ReassignMove(move);

            //tempField.ShowField(default, null);

            return tempField.IfKingCanBeHit(player, kingCord);
        }
        public List<(int, int)> GetDirectionsToCheck(Field field, (int, int) cord)
        {
            return field.AllCells[cord.Item1, cord.Item2].Figure is Rook ? _forwardDirection :
                    field.AllCells[cord.Item1, cord.Item2].Figure is Bishop ? _crossDirections :
                    field.AllCells[cord.Item1, cord.Item2].Figure is Queen ?
                    new List<(int, int)>().Concat(_forwardDirection).Concat(_crossDirections).ToList() : null;
        }
        public AllMoves GetMovesToMakeIfITsNotContainsInRays(Field field, (int, int) cord,
        (int, int) divison, Player player)
        {
            AllMoves res = new AllMoves();

            (int, int) temp = cord;

            do
            {
                temp = (temp.Item1 + divison.Item1, temp.Item2 + divison.Item2);

                if (field.IfChipIsOutOfRange(temp))
                {
                    return res;
                }
                else if (field.AllCells[temp.Item1, temp.Item2].Figure != null)
                {
                    if (field.AllCells[temp.Item1, temp.Item2].Figure.FigureColor != player.Color)
                    {
                        res.PossibleMoves.Add(new Move(new List<(int, int)>() { cord, temp },
                            new int[] { field.AllCells[cord.Item1,cord.Item2].Figure.FigureID,
                                field.AllCells[temp.Item1, temp.Item2].Figure.FigureID },
                                field.AllCells[temp.Item1, temp.Item2].Figure.GetCopy()));
                    }
                    return res;
                }
                else
                {
                    res.PossibleMoves.Add(new Move(new List<(int, int)>() { cord, temp }));
                }

            } while (true);
        }
        public List<(int, int)> GetDirections(Field field, (int, int) figCord,
        KingRays rays, Player player, (int, int) kingCord)
        {
            int amountofFigsThatCanHitKing = rays.FigsThatHitsKing.Count;
            List<(int, int)> toCheckDirections = GetDirectionsToCheck(field, figCord);

            if (amountofFigsThatCanHitKing == 1 &&
                !rays.FigsThatProtectsKing.Contains(figCord))
            {
                return GetOneDirectionIfNeedToProtectKing(toCheckDirections,
                    figCord, rays, player, field);
            }
            else if (amountofFigsThatCanHitKing == 0)
            {
                if (rays.FigsThatProtectsKing.Contains(figCord))
                {
                    (int, int) resdir = field.GetDirectionBetweenTwoFig(figCord, kingCord);// To check it

                    if ((field.AllCells[figCord.Item1, figCord.Item2].Figure is Queen) ||

                        (field.AllCells[figCord.Item1, figCord.Item2].Figure is Rook &&
                        _forwardDirection.Contains(resdir)) ||

                        (field.AllCells[figCord.Item1, figCord.Item2].Figure is Bishop &&
                        _crossDirections.Contains(resdir)))
                    {
                        return new List<(int, int)>() { resdir, field.GetOpositeDirection(resdir) };
                    }
                }
                else
                {
                    return toCheckDirections;
                }
            }
            return new List<(int, int)>();
        }
        public List<(int, int)> GetOneDirectionIfNeedToProtectKing(List<(int, int)> posDirections,
        (int, int) cordToCheck, KingRays kingRays, Player player, Field field)
        {
            List<List<(int, int)>> rayToProtectKing = GetRaysThatCanHitKing(kingRays.AllRays, player, field);

            return rayToProtectKing.Count > 1 ? new List<(int, int)>() :
                posDirections.Where(x => IfFigCanAchiveRayToProtectKing(rayToProtectKing, x, cordToCheck, field)).ToList();

        }
        public List<List<(int, int)>> GetRaysThatCanHitKing(List<List<(int, int)>> rays,
        Player player, Field field)
        {
            List<List<(int, int)>> hitRays = new List<List<(int, int)>>();
            for (int i = 0; i < rays.Count; i++)
            {
                for (int j = 0; j < rays[i].Count && !field.IfChipIsPlayers(player, rays[i][j]); j++)
                {
                    if (field.IfChipIsEnemys(player, rays[i][j]))
                    {
                        hitRays.Add(rays[i]);
                    }
                }
            }
            return hitRays;
        }
        public bool IfFigCanAchiveRayToProtectKing(List<List<(int, int)>> rays, (int, int) direction, (int, int) figCord, Field field)
        {
            do
            {
                figCord = (figCord.Item1 + direction.Item1, figCord.Item2 + direction.Item2);
                if (rays.Any(x => x.Contains(figCord)))
                {
                    return true;
                }
                else if (field.IfChipIsOutOfRange(figCord) || field.AllCells[figCord.Item1, figCord.Item2].Figure != null)
                {
                    return false;
                }

            } while (true);
        }
        public List<(int, int)> GetWayThatHitsPlayerKing(List<List<(int, int)>> allRays, Field field)
        {
            for (int i = 0; i < allRays.Count; i++)
            {
                bool check = true;
                for (int j = 0; j < allRays[i].Count; j++)
                {
                    if (field.AllCells[allRays[i][j].Item1, allRays[i][j].Item2].Figure != null && j != allRays[i].Count - 1)
                    {
                        check = false;
                    }
                }
                if (check)
                {
                    return allRays[i];
                }
            }
            return new List<(int, int)>();
        }
    }
}
