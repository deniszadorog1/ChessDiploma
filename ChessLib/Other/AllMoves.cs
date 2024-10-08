﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Other
{
    public class AllMoves
    {
        public List<Move> PossibleMoves { get; set; }

        public bool Contains(Move move)
        {
            return PossibleMoves.Any(x => x.Equals(move));
        }
        public AllMoves()
        {
            PossibleMoves = new List<Move>();
        }
        public AllMoves(List<Move> possibleMoves)
        {
            PossibleMoves = possibleMoves;
        }
    }
}
