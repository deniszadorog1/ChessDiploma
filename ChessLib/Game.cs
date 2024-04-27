﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using ChessLib.FieldModels;
using ChessLib.PlayerModels;
using ChessLib.Other;
using ChessLib.Figures;
using ChessLib.Enums.Players;
using ChessLib.Enums.Figures;
using ChessLib.Enums.Game;
using System.Reflection;

namespace ChessLib
{
    public class Game
    {
        public Field AllField { get; set; }
        public List<Player> Players { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public GameResult GameExodus { get; set; }

        public Player _steper;

        private int _timeInSeconds = 0;
        private int _time;

        private int _movesCounter = 0;
        private List<Move> _сheckForEqualMoves = new List<Move>();
        private int _movesWithoutHitCounter = 0;
        private int _toStartCheckForDraw = 12;
        private int _chosenEqualMovesToCheckForDraw = 4;
        private int _drawByMovesWithOutHitting = 50;




        public Game(Player firstPlayer, Player secondPlayer, DateTime start, DateTime end, GameResult exodus)
        {
            Players = new List<Player>() { firstPlayer, secondPlayer };
            AllField = new Field(Players);
            StartTime = start;
            EndTime = end;
            GameExodus = exodus;
            _steper = Players.Find(x => x.Color == PlayerColor.White);
        }
        public Game(Field allField, List<Player> players)
        {
            AllField = allField;
            Players = players;
            _steper = Players.Find(x => x.Color == PlayerColor.White);
        }
        public Game()
        {
            StartTime = DateTime.Now;
            Players = new List<Player>();
            AddPlaeyrs();
            AllField = new Field(Players);
            _steper = Players.Find(x => x.Color == PlayerColor.White);
        }
        public Game(Player player, Player enemy)
        {
            StartTime = DateTime.Now;

            Players = new List<Player>();
            Players.Add(player);
            Players.Add(enemy);

            AllField = new Field(Players);
            _steper = Players.Find(x => x.Color == PlayerColor.White);
        }
        public void AddPlaeyrs()
        {
            Players.Add(new User("first", PlayerColor.White, PlayerSide.Down,
                new List<(string name, int amount)>(), "", "", new DateTime(), -1, -1, -1, -1));
            Players.Add(new User("second", PlayerColor.Black, PlayerSide.Up,
                new List<(string name, int amount)>(), "", "", new DateTime(), -1, -1, -1, -1));
            _steper = Players.Find(x => x.Color == PlayerColor.White);
        }

        public int GetFieldLegthParam()
        {
            return AllField.AllCells.Length;
        }
        public int GetFieldSizeParam()
        {
            return (int)Math.Sqrt(AllField.AllCells.Length);
        }

        public Figure InitCord((int x, int y) cord)
        {
            return AllField.AllCells[cord.x, cord.y].Figure;
        }
        public AllMoves GetMovesForFigure((int, int) figCord)
        {
            return AllField.GetMovesForFigure(figCord, _steper);
        }
        public void ReassignMove(Move move)
        {
            AllField.ReassignMove(move);

            //Check if pawn went to the end
            AllField.IfPawnWentToTheEndOfField(move);
        }
        public void ChangeSteper()
        {
            if (_steper is User) ((User)_steper)._gameTimer.Stop();
            _steper = Players.Find(x => x.Color != _steper.Color);
            if (_steper is User) ((User)_steper)._gameTimer.Start();
        }
        public void StartFirstTimer()
        {
            Player player = Players.Find(x => x.Color == PlayerColor.White);

            if(!(player is null) && player is User)
            {
                ((User)player)._gameTimer.Start();
            }
        }
        public bool IfFigureIsStepers((int x, int y) cord)
        {
            return _steper.Side == AllField.AllCells[cord.x, cord.y].Figure.OwnerSide;
        }
        public bool IfSteperCheckMated()
        {
            return AllField.CheckForCheckMate(AllField, _steper);
        }
        public bool IfPawnCameToTheEndOfBoard()
        {
            return AllField.IfNeedToConvertFigure();
        }
        public int GetAmountFiguresThatCanBeConverted()
        {
            return (int)ConvertPawn.Bishop + 1;
        }
        public PlayerColor GetColorToConvert()
        {
            return _steper.Color;
        }
        public PlayerSide GetStepperSide()
        {
            return _steper.Side;
        }
        public (int, int) GetFiugerCordToConvert()
        {
            return AllField.GetFigureCordToConvert();
        }
        public void ConvertFigure((int x, int y) cord, ConvertPawn type)
        {
            AllField.ConvertFigure(cord, type);
        }
        public void ClearConvertationVariable()
        {
            AllField.ClearConvertationVariable();
        }
        public void IfSpecialFigIsMoves(Move move)
        {
            AllField.IfSpecialChipIsMoved(move);
        }
        public Player GetPlayer(int playerIndex)
        {
            return Players[playerIndex];
        }
        public (string, string) GetSteppersNameAdnColor()
        {
            return (_steper.Login, _steper.Color.ToString());
        }
        public void AddHitFigure(Figure hitFigure, int updater)
        {
            _steper.UpdateHitFigures(hitFigure, updater);
        }
        public int GetAmountOfHitFigureList()
        {
            return _steper.HitFigures.Count;
        }
        public (string name, int amount) GetPlayerHitFigure(int hitIndex)
        {
            return _steper.HitFigures[hitIndex];
        }
        public PlayerColor GetPlayerColor()
        {
            return _steper.Color;
        }
        public string GetFirstPlayerName()
        {
            return Players[0].Login;
        }
        public string GetLastPlayerName()
        {
            return Players[Players.Count - 1].Login;
        }
        public bool IfGameEndedByPate()
        {
            return AllField.IfEnemyCanMakeMoves(_steper);
        }
        public void AddMoveInHistory(Move move)
        {
            AllField.AddMoveInHistory(move);
        }
        public Move GetLastMove()
        {
            return AllField.GetLastMove();
        }
        public void DeleteLastMoveInHistory()
        {
            AllField.DeleteLastMoveInHistory();
        }
        public void DeclineLastMove()
        {
            AllField.DeclineMove();
        }
        public PlayerColor GetFigureColor((int, int) cord)
        {
            return AllField.AllCells[cord.Item1, cord.Item2].Figure.FigureColor;
        }
        public void AddFigToMoveInHistory(Move move)
        {
            AllField.AddMovedFigure(move);
        }
        public void DeleteFigToMove()
        {
            AllField.DeleteMovedFigure();
        }
        public bool IfCanDeclineMove()
        {
            return AllField.IfMoveCanBeDeclined();
        }
        public void InitTime(int time)
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if (Players[i] is User)
                {
                    ((User)Players[i]).startTime = time ;
                    ((User)Players[i])._currentTime = time ;
                }
            }
        }
        public string GetPlayerCurrentTime(int playerIndex)
        {
            return Players[playerIndex] is User ? ((User)Players[playerIndex]).GetTimerInString() : "";
        }
        public void InitTimers()
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if (Players[i] is User)
                {
                    ((User)Players[i]).InitTimer();
                }
            }
        }
        public int GetTime()
        {
            return _time;
        }
        public void InitSteper()
        {
            _steper = Players.Find(x => x.Color == PlayerColor.White);
        }
    }
}
