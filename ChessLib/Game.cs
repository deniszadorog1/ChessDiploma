using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Reflection;

using ChessLib.FieldModels;
using ChessLib.PlayerModels;
using ChessLib.Other;
using ChessLib.Figures;
using ChessLib.Enums.Players;
using ChessLib.Enums.Figures;
using ChessLib.Enums.Game;
using ChessLib.Enums.Field;

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

        private int _time;

        private const int _stateScoreEarnForGame = 8;
        private const int _muchMoreWinnerDifferance = 4;
        private const int _muchMorePointsLosser = 2;

        private const int _muchMorePointWinner = _stateScoreEarnForGame - _muchMoreWinnerDifferance;

        private const int _dotsInUsualMove = 2;

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
            AllMoves moves = AllField.GetMovesForFigure(figCord, _steper);

            for (int i = 0; i < moves.PossibleMoves.Count; i++)
            {
                moves.PossibleMoves[i].ChangeSteperColor(_steper.Color);
            }

            return moves;
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

            if (!(player is null) && player is User)
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
            GetPlayerThatHitFigure(hitFigure).UpdateHitFigures(hitFigure, updater);
            //_steper.UpdateHitFigures(hitFigure, updater);
        }
        public Player GetPlayerThatHitFigure(Figure figure)
        {
            return Players.Find(x => x.Color != figure.FigureColor);
        }
        public int GetAmountOfHitFigureList()
        {
            return _steper.HitFigures.Count;
        }
        public (string name, int amount) GetPlayerHitFigure(int hitIndex, Player player)
        {
            return player.HitFigures[hitIndex];
        }
        public PlayerColor GetPlayerColor()
        {
            return _steper.Color;
        }
        public string GetFirstPlayerName()
        {
            return Players[0].Login;
        }
        public string GetSecondPlayerName()
        {
            return Players[1].Login;
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
        public void DeclineMove(Move move)
        {
            AllField.DeclineMove(move);
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
            _time = time;
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i] is User)
                {
                    ((User)Players[i]).startTime = time;
                    ((User)Players[i])._currentTime = time;
                }
            }
        }
        public string GetPlayerCurrentTime(int playerIndex)
        {
            return Players[playerIndex] is User ? ((User)Players[playerIndex]).GetTimerInString() : "";
        }
        public int GetTime()
        {
            return _time;
        }
        public void InitSteper()
        {
            _steper = Players.Find(x => x.Color == PlayerColor.White);
        }

        public void StepperPushedGIveupButton()
        {
            //Init game into db
            //init movesInto DB
        }
        public Player GetAnoutherPlayer()
        {
            return Players.Find(x => x.Login != _steper.Login);
        }

        public void UpdetePlayersWhenSteperGaveUp()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i] is User)
                {
                    if (Players[i].Login == _steper.Login)
                    {
                        ((User)_steper).Losts++;
                    }
                    else //winner
                    {
                        ((User)Players[i]).Wons++;
                    }
                }
            }
        }
        public void AssignTimeOnTimerInMove(Move move)
        {
            if (_steper is User)
            {
                move.AssignTime(((User)_steper).GetCurrnetTimeOnTimer());
            }
        }
        public void InitCastlingType(Move move)
        {
            if (move.OneMove.Count == _dotsInUsualMove) return;

            if (move.OneMove[0].Item2 - move.OneMove[2].Item2 != -3) move.InitCastling(CastlingType.Long); 
            else move.InitCastling(CastlingType.Short);
        }

        public  void StopTimers()
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if (Players[i] is User)
                {
                    ((User)Players[i]).StopTimer();
                }
                
            }
        }
        public List<Move> GetMoveHistory()
        {
            return AllField.GetMoveHistory();
        }
        public void InitMoveHistory(List<Move> moves)
        {
            AllField.InitMovesHistory(moves);
        }
        public Move GetMoveForBot()
        {
            return ((Bot)_steper).GetMove(new Field(AllField), _steper, null, 0);
        }
        public bool IfSteperIsBot()
        {
            return _steper is Bot;
        }
        public int GetMoveIndexForReplay()
        {
            return AllField.GetMoveIndexForReplay();
        }
        public void NextMoveForReplay()
        {
            AllField.NextMoveForReplay();
        }
        public void PreviousMoveForReplay()
        {
            AllField.PreviousMoveForReplay();
        }
        public bool IfCanGetNextMove()
        {
            return AllField.IfCanGetNextMove();
        }
        public bool IfCanGetPreviousMove()
        {
            return AllField.IfCanGetPreviousMove();
        }
        public Move GetMoveForReplay()
        {
            return AllField.GetMoveForReplay();
        }
        public Move GetMoveFromHistory(int index)
        {
            return AllField.GetMoveFromHistory(index);
        }
        public void AddMovedFigureInDisplayMode(Move move, int moveIndex)
        {
            AllField.AddMovesFigureInReplayMode(move, moveIndex);
        }
        public void GameEndedByDraw()
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if (Players[i] is User)
                {
                    ((User)Players[i]).Draws++;
                }
            }
        }
        public void StepperWonTheGame()
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if (Players[i] is User)
                {
                    if (Players[i].Login == _steper.Login)
                    {
                        ((User)Players[i]).Wons++;
                    }
                    else
                    {
                        ((User)Players[i]).Losts++;
                    }
                }
            }
        }
        public void ClearPlayersHitLists()
        {
            for(int i = 0; i < Players.Count; i++)
            {
                Players[i].ClearHitList();
            }
        }
        public bool IfItsDrawByEqualMoves()
        {
            return AllField.IfItsDrawByThreeEqualMoves();
        }
        public bool IfItsDrawByMovesWithoutHitting()
        {
            return AllField.IfItsDrawByMovesWithoutHit();
        }
        public void InitWinnerGameResult()
        {
            GameExodus = _steper.Login == Players[0].Login ? 
                GameResult.FirstWon : GameResult.SecondWon;
        }
        public void InitResultSteperGaveUp()
        {
            GameExodus = _steper.Login == Players[0].Login ?
                GameResult.SecondWon : GameResult.FirstWon;
        }
        public void InitGameResultDraw()
        {
            GameExodus = GameResult.Draw;
        }
        public void InitGameResultGameClosed()
        {
            GameExodus = GameResult.Closed;
        }
        public bool IfStepersFigIsInCell((int,int) cord)
        {
            return AllField.IfSteppersFigIsInUnit(cord, _steper);
        }


        public (int,int) CalcPlayersRainting()
        {
            if (GameExodus == GameResult.Draw || !(Players[0] is User) || !(Players[1] is User)) return (-1, -1);

            //Lost which has much more points < 20
            //Lost which has a little more points  20 > x > 0 
            //Lost which has a little less point (20 > x > 0)
            //Lost which has a much les points (20+)

            (int, int) res = (-1, -1);
            if(GameExodus == GameResult.FirstWon)
            {
                res = RaintingAddingTable((User)Players[0], (User)Players[1]);
                CheckForRaitingLowerThenZero();
                return res;
            }
            else if(GameExodus == GameResult.SecondWon)
            {
                res = RaintingAddingTable((User)Players[1], (User)Players[0]);
                CheckForRaitingLowerThenZero();
                return (res.Item2, res.Item1);
            }
            else if(GameExodus == GameResult.Draw)
            {
                return (0, 0);
            }
            CheckForRaitingLowerThenZero();
            return (-1, -1);
        }
        public void CheckForRaitingLowerThenZero()
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if (((User)Players[i]).Rating < 0)
                {
                    ((User)Players[i]).Rating = 0;
                }
            }
        }
        public (int, int) RaintingAddingTable(User winner, User losser)
        {
            int differance = winner.Rating - losser.Rating;
            (int, int) res = (-1, -1);
            if (differance >= 20)
            {
                res = WinnerHasMuchMoreRaitingThenLosser(winner, losser);
            }
            else if (differance >= 5)
            {
                res = WinnerHasLittleMorePoints(winner, losser);
            }
            else if (differance <= 5 && differance >= -5)
            {
                res = WinnerAndLosserHasEqualPoints(winner, losser);
            }
            else if (differance < -5)
            {
                res = WinnerHadLessPointsThanLosser(winner, losser);
            }
            return res;
        }

        public (int,int) WinnerHasMuchMoreRaitingThenLosser(User winner, User losser)
        {
            ((User)winner).Rating += _muchMorePointWinner;
            ((User)losser).Rating -= _muchMorePointsLosser;
            return (_muchMorePointWinner, -_muchMorePointsLosser);
        }
        public (int, int) WinnerAndLosserHasEqualPoints(User winner, User losser)
        {
            winner.Rating += _stateScoreEarnForGame;
            losser.Rating -= _stateScoreEarnForGame;
            return (_stateScoreEarnForGame, -_stateScoreEarnForGame);
        }
        public (int, int) WinnerHasLittleMorePoints(User winner, User losser)
        {
            winner.Rating += _stateScoreEarnForGame - _muchMorePointsLosser;
            losser.Rating -= _muchMoreWinnerDifferance;
            return (_stateScoreEarnForGame - _muchMorePointsLosser, -_muchMoreWinnerDifferance);
        }
        public (int, int) WinnerHadLessPointsThanLosser(User winner, User losser)
        {
            winner.Rating += _stateScoreEarnForGame + _muchMoreWinnerDifferance;
            losser.Rating -= _stateScoreEarnForGame + _muchMorePointsLosser;
            return (_stateScoreEarnForGame + _muchMoreWinnerDifferance, 
                -_stateScoreEarnForGame - _muchMorePointsLosser);
        }
    }
}
