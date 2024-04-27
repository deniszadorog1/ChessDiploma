using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Figures;
using ChessLib.Figures;

namespace ChessLib.Other
{
    public class Move
    {
        public List<(int, int)> OneMove { get; set; }
        public int[] HitHistIDs { get; set; } = new int[2];
        public ConvertPawn? ConvertFigure { get; set; }
        public (int, int)? HitCellCordForBeatingOnThePass { get; set; }
        public Figure HitFigure { get; set; }

        private List<char> _lettersToSave = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        public Move(List<(int, int)> oneMove, int[] arr, Figure hitFigure)
        {
            OneMove = oneMove;
            HitHistIDs = arr;
            HitFigure = hitFigure;
        }
        public Move(List<(int, int)> oneMove, ConvertPawn convertFigure, int[] arr, Figure hitFigure)
        {
            OneMove = oneMove;
            ConvertFigure = convertFigure;
            HitHistIDs = arr;
            HitFigure = hitFigure;
        }
        public Move(List<(int, int)> oneMove, ConvertPawn convertFigure)
        {
            OneMove = oneMove;
            ConvertFigure = convertFigure;
        }

        public Move(List<(int, int)> oneMove)
        {
            OneMove = oneMove;
        }

        public Move(List<(int, int)> history, int[] hitHistIDs)
        {
            OneMove = history;
            HitHistIDs = hitHistIDs;
        }
        public Move()
        {
            OneMove = new List<(int, int)>();
            ConvertFigure = null;
            HitCellCordForBeatingOnThePass = null;
            HitHistIDs[0] = -1;
            HitHistIDs[1] = -1;
            HitFigure = null;
        }
        public override bool Equals(object obj)
        {
            Move temp = obj as Move;

            if (!OneMove.SequenceEqual(temp.OneMove)) return false;
            else if (temp.ConvertFigure != ConvertFigure) return false;
            else if (temp.HitCellCordForBeatingOnThePass != HitCellCordForBeatingOnThePass) return false;
            else if (!temp.HitHistIDs.SequenceEqual(HitHistIDs)) return false;

            return true;
        }
        public char GetConvertedChar(int number)
        {
            return _lettersToSave[number];
        }
    }
}
