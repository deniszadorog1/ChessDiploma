using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Figures;
using ChessLib.Figures;
using ChessLib.Enums.Players;
using ChessLib.Enums.Field;

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
        private int _timerOnMove = -1;
        private PlayerColor _playersColor;
        private CastlingType? _castling = null;

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
        public PlayerColor GetPlayerColor()
        {
            return _playersColor;
        }
        public int GetTimeOnTimer()
        {
            return _timerOnMove;
        }
        public CastlingType? GetCastlingType()
        {
            return _castling;
        }
        public void ChangeSteperColor(PlayerColor color)
        {
            _playersColor = color;
        }
        public void AssignTime(int time)
        {
            _timerOnMove = time;
        }
        public void InitCastling(CastlingType castling)
        {
            _castling = castling;
        }

        public void InitMoveInCastling(CastlingType type, PlayerSide side)
        {
            if (type == CastlingType.Short && side == PlayerSide.Up)
            {
                OneMove = new List<(int, int)>() { (0, 4), (0, 6), (0, 7), (0, 5) };
            }
            else if (type == CastlingType.Short && side == PlayerSide.Down)
            {
                OneMove = new List<(int, int)>() { (7, 4), (7, 6), (7, 7), (7, 5) };
            }
            else if (type == CastlingType.Long && side == PlayerSide.Up)
            {
                OneMove = new List<(int, int)>() { (0, 4), (0, 2), (0, 0), (0, 3) };
            }
            else if (type == CastlingType.Long && side == PlayerSide.Down)
            {
                OneMove = new List<(int, int)>() { (7, 4), (7, 2), (7, 0), (7, 3) };
            }
        }
    }
}
