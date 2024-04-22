using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;

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

        public Bot(string name, PlayerColor playerColor, PlayerSide side) :
            base(name, playerColor, side)
        {
            rnd = new Random();
        }
        public Bot() : base()
        {
            rnd = new Random();
        }
    }
}
