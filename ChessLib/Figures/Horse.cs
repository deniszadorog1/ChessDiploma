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
    public class Horse : Figure
    {
        public List<(int, int)> DivisionsToStep = new List<(int, int)>()
        {
            (-1, -2),
            (-2, -1),

            (-2, 1),
            (-1, 2),

            (1, 2),
            (2, 1),

            (2, -1),
            (1, -2)
        };

        private readonly double[,] posTableDownPlayer = new double[8, 8] {
            {-0.5, -0.4, -0.3, -0.3, -0.3, -0.3, -0.4, -0.5},
            {-0.4, -0.2, 0, 0, 0, 0, -0.2, -0.4},
            {-0.3, 0, 0.1, 0.15, 0.15, 0.1, 0, -0.3},
            {-0.3, 0.05, 0.15, 0.2, 0.2, 0.15, 0.05, -0.3},
            {-0.3, 0, 0.15, 0.2, 0.2, 0.15, 0, -0.3},
            {-0.3, 0.05, 0.1, 0.15, 0.15, 0.1, 0.05, -0.3},
            {-0.4, -0.2, 0, 0.05, 0.05, 0, -0.2, -0.4},
            {-0.5, -0.4, -0.3, -0.3, -0.3, -0.3, -0.4, -0.5}
        };
        public Horse(PlayerColor figColor, int figureId, (int,int) figCord, PlayerSide ownerSide) :
            base(figColor, figureId, figCord, ownerSide)
        {

        }
        public Horse()
        {
            FigureColor = new PlayerColor();
        }
        public override Figure GetCopy()
        {
            return new Horse(FigureColor, FigureID, FigureCord, OwnerSide);
        }
        public override double GetScoreForFigure()
        {
            return _scoreForHorse;
        }
        public override AllMoves GetHitMoves(Field field, Player player, (int, int) figCord)
        {
            AllMoves res = new AllMoves();

            for (int i = 0; i < DivisionsToStep.Count; i++)
            {
                (int, int) check = (figCord.Item1 + DivisionsToStep[i].Item1, figCord.Item2 + DivisionsToStep[i].Item2);

                if (!field.IfChipIsOutOfRange(check) &&
                    field.IfChipIsEnemys(player, check))
                {
                    res.PossibleMoves.Add(new Move(new List<(int, int)>() { figCord, check },
                    new int[] { FigureID, field.AllCells[check.Item1, check.Item2].Figure.FigureID }));
                }
            }
            return res;
        }
        public override double GetScoreForFigPositionOnBoard((int, int) cord)
        {
            return OwnerSide == PlayerSide.Up ? posTableDownPlayer[cord.Item1, cord.Item2] :
            posTableDownPlayer[_maxCordOnBoard - cord.Item1, _maxCordOnBoard - cord.Item2];
        }
        public override AllMoves GetMoves(Player player,
        (int, int) startCord, Field field, KingRays kingRays)
        {
            return (kingRays.FigsThatHitsKing.Count == 1 && !kingRays.FigsThatProtectsKing.Contains(startCord)) ?
                GetMovesThatCanProtectKing(kingRays.AllRays, startCord, field) :

                (kingRays.FigsThatHitsKing.Count == 0 && !kingRays.FigsThatProtectsKing.Contains(startCord)) ?
                GetAllMoves(field, startCord, player) : new AllMoves();
        }
        public AllMoves GetMovesThatCanProtectKing(List<List<(int, int)>> rays,
            (int, int) horseCord, Field field)
        {
            AllMoves res = new AllMoves();

            List<(int, int)> hitRay = GetWayThatHitsPlayerKing(rays, field);

            for (int i = 0; i < DivisionsToStep.Count; i++)
            {
                (int, int) cordToStepOn = (horseCord.Item1 + DivisionsToStep[i].Item1,
                           horseCord.Item2 + DivisionsToStep[i].Item2);
                if (hitRay.Contains(cordToStepOn) &&
                    !field.IfChipIsOutOfRange(cordToStepOn))
                {
                    if (field.AllCells[cordToStepOn.Item1, cordToStepOn.Item2].Figure == null)
                    {
                        res.PossibleMoves.Add(new Move(new List<(int, int)>() { horseCord, cordToStepOn }));
                    }
                    else
                    {
                        res.PossibleMoves.Add(new Move(new List<(int, int)>() { horseCord, cordToStepOn },
                            new int[] { FigureID, field.AllCells[cordToStepOn.Item1, cordToStepOn.Item2].Figure.FigureID },
                            field.AllCells[cordToStepOn.Item1, cordToStepOn.Item2].Figure.GetCopy()));
                    }
                }
            }
            return res;
        }
        public AllMoves GetAllMoves(Field field, (int, int) horseCord, Player player)
        {
            AllMoves res = new AllMoves();

            for (int i = 0; i < DivisionsToStep.Count; i++)
            {
                (int, int) cordToStepOn = (horseCord.Item1 + DivisionsToStep[i].Item1,
                           horseCord.Item2 + DivisionsToStep[i].Item2);
                if (!field.IfChipIsOutOfRange(cordToStepOn) &&
                    !field.IfChipIsPlayers(player, cordToStepOn))
                {
                    if (field.AllCells[cordToStepOn.Item1, cordToStepOn.Item2].Figure == null)
                    {
                        res.PossibleMoves.Add(new Move(new List<(int, int)>() { horseCord, cordToStepOn }));
                    }
                    else
                    {
                        res.PossibleMoves.Add(new Move(new List<(int, int)>() { horseCord, cordToStepOn },
                            new int[] { field.AllCells[horseCord.Item1, horseCord.Item2].Figure.FigureID,
                            field.AllCells[cordToStepOn.Item1, cordToStepOn.Item2].Figure.FigureID},
                            field.AllCells[cordToStepOn.Item1, cordToStepOn.Item2].Figure.GetCopy()));
                    }
                }
            }
            return res;
        }
    }
}
