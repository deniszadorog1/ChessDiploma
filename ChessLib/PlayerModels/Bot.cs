﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;
using ChessLib.Other;
using ChessLib.FieldModels;
using ChessLib.Figures;

namespace ChessLib.PlayerModels
{
    public class Bot : Player
    {
        private const int _depth = 3;
        private Random rnd;

        private const int _upperFloorToTakeRandomMove = 1;

        private const int _startAlphaValue = -9000;
        private const int _startBetaValue = 9000;
        private const int _checkMatePoint = 10000;
        private const int _minEvalForTempDepth = -5000;
        private const int _maxEvalForTempDepth = 5000;

        private const int _hitMovePointsAmount = 2;
        private const int _depthToForHittingEachOther = 2;

        public Bot(string name, PlayerColor playerColor, PlayerSide side, List<(string name, int amount)> hitFigures) :
            base(name, playerColor, side, hitFigures)
        {
            rnd = new Random();
        }
        public Bot() : base()
        {
            rnd = new Random();
        }

        public Move GetMove(Field field, Player player,
        Move drawMove, int movesCounter)
        {
            #region Get move for bot

            AllMoves movesToCheck = field.GetSortedMoves(player);
            if (drawMove != null)
            {
                movesToCheck = DeleteMoveIfItsGoesToDraw(movesToCheck, drawMove);
            }
            if (movesCounter < _upperFloorToTakeRandomMove)
            {
                return movesToCheck.PossibleMoves[rnd.Next(0, movesToCheck.PossibleMoves.Count - 1)];
            }
            if (movesToCheck.PossibleMoves.Count == 1)
            {
                return movesToCheck.PossibleMoves.First();
            }

            return GetTheBestMoveAsyncTest(field, player, movesToCheck).Result;
            #endregion
        }
        public AllMoves DeleteMoveIfItsGoesToDraw
        (AllMoves allMoves, Move drawMove)
        {
            if (allMoves.Contains(drawMove))
            {
                AllMoves res = new AllMoves();

                res.PossibleMoves = allMoves.PossibleMoves.Where(x => !x.Equals(drawMove)).ToList();

                return res;
            }
            return allMoves;
        }
        public async Task<Move> GetTheBestMoveAsyncTest(Field field,
            Player player, AllMoves moves)
        {
            object locker = new object();

            List<double> scores = new List<double>();

            List<Task<double>> test = new List<Task<double>>();
            List<(int, double)> results = new List<(int, double)>();

            DateTime start = DateTime.Now;

            for (int i = 0; i < moves.PossibleMoves.Count; i++)
            {
                int index = i;

                test.Add(Task.Run(async () =>
                {
                    DateTime startTask = DateTime.Now;
                    //Console.WriteLine(index);

                    double result = await GetScoreForMove
                    (field, player, moves.PossibleMoves[index]);

                    lock (locker)
                    {
                        results.Add((index, result));
                    }
                    //Console.WriteLine(DateTime.Now - startTask + "  Pot end index: " + index);
                    return result;
                }));
            }

            await Task.WhenAll(test);

            return GetResMoves(results, moves).PossibleMoves.First();
        }

        public AllMoves GetResMoves(List<(int, double)> indexAndScoreForMoves, AllMoves allMoves)
        {
            AllMoves resMoves = new AllMoves();
            double maxScore = int.MinValue;
            maxScore = indexAndScoreForMoves.Max(x => x.Item2);
            resMoves.PossibleMoves = indexAndScoreForMoves.Where(x => x.Item2 == maxScore).
                Select(x => allMoves.PossibleMoves[x.Item1]).ToList();
            return resMoves;
        }
        public async Task<double> GetScoreForMove(Field field, Player player, Move move)
        {
            Field tempField = new Field(field);

            tempField.ReassignMove(move);
            tempField.IfSpecialChipIsMoved(move);

            return GetScoresForMovesInDepth(tempField,
                _depth - 1, false, _startAlphaValue, _startBetaValue, player, field.GetAnoutherPlayer(player), null);
        }
        public double GetScoresForMovesInDepth(Field field, int depth, bool minOrMaxValue,
            double alpha, double beta, Player startPlayer, Player tempPlayer, AllMoves hitsToCheckAtTheEnd)
        {
            //Transpo table logic.
            //For one player only in last depth can be eqaul field(field, depth, to make move order).

            //ulong key = table.GeneratePositionKey(field, depth);

            /*
                        if (table.IsKeyContainsInTable(key, out (double, int) value))
                        {
                            return value.Item1;
                        }*/

            AllMoves allMoves = field.GetSortedMoves(tempPlayer);

            if (depth == 0 || allMoves.PossibleMoves.Count == 0)
            {
                double res = field.GetScoreForExodusVersionTwo(tempPlayer, startPlayer, allMoves.PossibleMoves.Count);

                AllMoves hitMoves = field.GetUniqueHitMovesFromOtherMoves(allMoves, hitsToCheckAtTheEnd);

                if (hitMoves.PossibleMoves.Count > 0)
                {
                    double depthHitting = GetScoreForHitEachOtherAtTheEnd(field, startPlayer, hitMoves,
                        field.GetAnoutherPlayer(tempPlayer), _depthToForHittingEachOther, minOrMaxValue, alpha, beta);

                    if (depthHitting < res)
                    {
                        res = depthHitting;
                    }
                }

                //table.WriteInTable(key, (res, depth));
                return res;
            }
            if (startPlayer != tempPlayer)
            {
                hitsToCheckAtTheEnd = field.GetHitMoves(allMoves);
            }
            double minMaxEval = minOrMaxValue ? _minEvalForTempDepth : _maxEvalForTempDepth;

            for (int i = 0; i < allMoves.PossibleMoves.Count; i++)
            {
                Field tempField = new Field(field);

                tempField.ReassignMove(allMoves.PossibleMoves[i]);

                tempField.IfSpecialChipIsMoved(allMoves.PossibleMoves[i]);

                double eval = GetScoresForMovesInDepth(tempField, depth - 1, !minOrMaxValue,
                    alpha, beta, startPlayer, tempField.GetAnoutherPlayer(tempPlayer), hitsToCheckAtTheEnd);

                if ((minOrMaxValue && eval > minMaxEval) ||
                    (!minOrMaxValue && eval < minMaxEval))
                {
                    minMaxEval = eval;
                }
                if (minOrMaxValue)
                {
                    alpha = Math.Max(alpha, eval);
                }
                else
                {
                    beta = Math.Min(beta, eval);
                }
                if (beta <= alpha)
                {
                    break;
                }
            }
            return minMaxEval;
        }
        public double GetScoreForHitEachOtherAtTheEnd(Field field, Player startPlayer,
    AllMoves movesToCheck,
    Player tempPlayer, int hitEachOtherDepth, bool minOrMax, double alpha, double beta)
        {
            if (movesToCheck.PossibleMoves.Count == 0 || hitEachOtherDepth == 0)
            {
                return field.GetScoreForField(startPlayer);
            }

            AllMoves allEnemysHitMoves = field.GetHitMovesVersionTwo(tempPlayer);
            double minMaxEval = minOrMax ? _minEvalForTempDepth : _maxEvalForTempDepth;

            for (int i = 0; i < movesToCheck.PossibleMoves.Count; i++)
            {
                (int, int) checker = movesToCheck.PossibleMoves[i].OneMove.Last();
                if (field.AllCells[checker.Item1, checker.Item2].Figure.GetType() != typeof(King))
                {
                    Field tempField = new Field(field);

                    tempField.ReassignMove(movesToCheck.PossibleMoves[i]);

                    double eval = GetScoreForHitEachOtherAtTheEnd(tempField, startPlayer,

                        tempField.GetUniqueHitMovesFromOtherMoves(tempField.GetHitMovesVersionTwo(tempPlayer), allEnemysHitMoves),

                        tempField.GetAnoutherPlayer(tempPlayer), hitEachOtherDepth - 1, !minOrMax, alpha, beta);

                    if ((minOrMax && eval > minMaxEval) ||
                    (!minOrMax && eval < minMaxEval))
                    {
                        minMaxEval = eval;
                    }

                    if (minOrMax)
                    {
                        alpha = Math.Max(alpha, eval);
                    }
                    else
                    {
                        beta = Math.Min(beta, eval);
                    }
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

            }
            return minMaxEval;
        }
    }
}
