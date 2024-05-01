using System;
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
        private const int _depth = 1;
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
            if (movesCounter + 2 < _upperFloorToTakeRandomMove)
            {
                return movesToCheck.PossibleMoves[rnd.Next(0, movesToCheck.PossibleMoves.Count - 1)];
            }
            /*            if (movesToCheck.PossibleMoves.Count == 1)
                        {
                            return movesToCheck.PossibleMoves.First();
                        }*/

            return GetTheBestMove(field, player, movesToCheck);
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
            double minMaxEval = minOrMaxValue ? _minEvalForTempDepth : 
                _maxEvalForTempDepth;

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

        /// <summary>
        /// Get the best move by sync system
        /// </summary>
        /// <param name="field">field to get move on</param>
        /// <param name="player">player to get moves for</param>
        /// <param name="moves">moves to get moves on</param>
        /// <param name="table">transpo table</param>
        /// <returns>best move</returns>
        public Move GetTheBestMove(Field field, Player player, AllMoves moves)
        {
            double mostScore = int.MinValue;
            Move bestMove = null;
            AllMoves bestMoves = new AllMoves();
            double scoreForMove;

            DateTime start = DateTime.Now;

            for (int i = 0; i < moves.PossibleMoves.Count; i++)
            {
                DateTime temp = DateTime.Now;

                Field tempField = new Field(field);

                tempField.ReassignMove(moves.PossibleMoves[i]);
                tempField.IfSpecialChipIsMoved(moves.PossibleMoves[i]);

                //tempField.ShowField(default, null);

                scoreForMove = GetScoresForMovesInDepth(tempField,
                    _depth - 1, false, _startAlphaValue, _startBetaValue, player, field.GetAnoutherPlayer(player), null);

                if (scoreForMove >= mostScore)
                {
                    if (scoreForMove > mostScore)
                    {
                        mostScore = scoreForMove;
                        bestMoves = new AllMoves(new List<Move>() { moves.PossibleMoves[i] });

                        if (scoreForMove >= _checkMatePoint)
                        {
                            return moves.PossibleMoves[i];
                        }
                    }
                    else
                    {
                        mostScore = scoreForMove;
                        bestMoves.PossibleMoves.Add(moves.PossibleMoves[i]);
                    }
                }
                DateTime endTemp = DateTime.Now;
                //Console.WriteLine(endTemp - temp);
            }

            DateTime endEnd = DateTime.Now;
            Console.WriteLine(endEnd - start);

            //Console.WriteLine(allCounter);

            if (bestMoves.PossibleMoves.Count > 0)
            {
                AllMoves test = DeleteAllNotHitMoves(field, bestMoves);

                //bestMove = bestMoves.PossibleMoves[rnd.Next(bestMoves.PossibleMoves.Count)];
                bestMove = test.PossibleMoves[rnd.Next(test.PossibleMoves.Count)];
                //field.IfSpecialChipIsMoved(bestMove.OneMove.Fi);
            }

            DateTime end = DateTime.Now;

            Console.WriteLine(end - start);

            return bestMove;
        }
        /// <summary>
        /// Shows move history
        /// </summary>
        /// <param name="move">move to show</param>
        public void ShowOneMove(Move move)
        {
            Console.Write("(");
            for (int i = 0; i < move.OneMove.Count; i++)
            {
                Console.Write(move.OneMove[i].Item1 + " " + move.OneMove[i].Item2 + " , ");
            }
            Console.Write(")");
            Console.WriteLine();
        }
        /// <summary>
        /// If there is hit moves, we get only them
        /// </summary>
        /// <param name="field">field to get moves from</param>
        /// <param name="allMoves">moves to check</param>
        /// <returns>hit moves</returns>
        public AllMoves DeleteAllNotHitMoves(Field field, AllMoves allMoves)
        {
            AllMoves moves =
              new AllMoves(allMoves.PossibleMoves.Where(p => p.OneMove.Count == _hitMovePointsAmount &&
              field.AllCells[p.OneMove.Last().Item1, p.OneMove.Last().Item2].Figure != null).ToList());

            return moves.PossibleMoves.Count > 0 ? moves : allMoves;
        }
    }
}
