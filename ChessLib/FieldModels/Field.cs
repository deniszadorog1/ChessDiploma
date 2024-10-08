﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using ChessLib.PlayerModels;
using ChessLib.Figures;
using ChessLib.Enums.Players;
using ChessLib.Other;
using ChessLib.Enums.Figures;

namespace ChessLib.FieldModels
{
    public class Field
    {
        public Cell[,] AllCells { get; set; }
        private List<Player> _players = new List<Player>();

        private List<Move> _movesHistory = new List<Move>();
        private List<Figure> _movedFigsInHistory = new List<Figure>();
        private int _moveIndexForReplay = -1;

        private const int _fieldHeight = 8;
        private const int _fieldWidth = 8;
         
        private KingRays _rays = new KingRays();

        private List<(int, int)> _secondPlayerPawnCords = new List<(int, int)>()
        {(1,0),(1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7)};
        private List<(int, int)> _secondPlayerRookCords = new List<(int, int)>()
        {(0,0),(0,7)};
        private List<(int, int)> _secondPlayerHorseCords = new List<(int, int)>()
        {(0,1),(0,6)};
        private List<(int, int)> _secondPlayerBishopCords = new List<(int, int)>()
        {(0,2),(0,5)};
        private (int, int) _secondPlayerQueenCord = (0, 3);
        private (int, int) _secondPlayerKingCord = (0, 4);

        private List<(int, int)> _firstPlayerPawnCords = new List<(int, int)>()
        {(6,0),(6,1),(6,2),(6,3),(6,4),(6,5),(6,6),(6,7)};
        private List<(int, int)> _firstPlayerRookCords = new List<(int, int)>()
        {(7,0),(7,7)};
        private List<(int, int)> _firstPlayerHorseCords = new List<(int, int)>()
        {(7,1),(7,6)};
        private List<(int, int)> _firstPlayerBishopCords = new List<(int, int)>()
        {(7,2),(7,5)};

        private List<(int, int)> _crossDirection = new List<(int, int)>()
        {(-1, 1), (1,1), (1,-1), (-1, -1) };
        private List<(int, int)> _forwardDirection = new List<(int, int)>()
        {(-1, 0), (0, 1), (1,0), (0,-1) };
        private List<(int, int)> _horseDirectinos = new List<(int, int)>()
        {(-2, -1), (-1, -2), (1, -2), (2, -1), (2,1), (1,2), (-1, 2), (-2, 1) };
        private readonly List<(int, int)> _pawnHitDirectionForDownSidePlayer = new List<(int, int)>() { (-1, -1), (-1, 1) };
        private readonly List<(int, int)> _pawnHitDirectionForUpSidePlayer = new List<(int, int)>() { (1, -1), (1, 1) };
        private readonly List<(int, int)> _kingHitMoveDirections = new List<(int, int)>()
            {(-1,0), (-1, 1), (0,1), (1,1), (1,0), (1,-1), (0, -1), (-1,-1) };

        private (int, int) _firstPlayerQueenCord = (7, 3);
        private (int, int) _firstPlayerKingCord = (7, 4);

        private List<(int, int)> _centralCell = new List<(int, int)>()
        { (3,3), (3,4), (4,3), (4,4)};

        private int _tempFigureId = 0;
        //Showr Casling
        private const int _firstInShortCastling = 0;
        private const int _secondInShortCastling = 2;
        private const int _thirdInShortCastling = 3;
        private const int _forthInShortCastling = 1;
        //Long Castling
        private const int _firstInLongCastling = 0;
        private const int _secondInLongCastling = 2;
        private const int _thirdInLongCastling = 4;
        private const int _forthInLongCastling = 1;


        private const int _strightBackDirection = -1;
        private const int _straightForwarDirection = 1;
        private const int _standOnPlaceDirection = 0;


        private const int _valueForWonMate = 10000;
        private const int _valueForLostMate = -10000;

        private readonly List<(int, int)> _castlingDirections = new List<(int, int)>() { (0, 1), (0, -1) };
        private int _shortCastling = 4;
        private int _longCastling = 5;

        private bool _ifNeedToConvertFig = false;
        private Figure _movedFigureForDecline = null;
        private double _scoreForMovedKing = 0.01;

        private const int _movesWithoutHotCounter = 50;
        private const int _moveToTakeToCheckForEqaulDraw = 12;
        private const int _moveToDrawByEqualMoves = 3;

        public Field(List<Player> players)
        {
            _players = players;
            AllCells = new Cell[_fieldHeight, _fieldWidth];

            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    AllCells[i, j] = new Cell();

                    AllCells[i, j].Figure = GetFigureToFill((i, j));

                    if (!(AllCells[i, j].Figure is null))
                    {
                        _tempFigureId++;
                    }

                }
            }
        }
        public Field(Field copyField)
        {
            _players = new List<Player>()
            {
                new Player(copyField._players[0].Login ,copyField._players[0].Color ,copyField._players[0].Side, copyField._players[0].HitFigures),
                new Player(copyField._players[1].Login, copyField._players[1].Color, copyField._players[1].Side, copyField._players[1].HitFigures)
            };

            AllCells = new Cell[_fieldHeight, _fieldWidth];

            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    AllCells[i, j] = new Cell(copyField.AllCells[i, j].Color,
                        copyField.AllCells[i, j].Figure is null ? null : copyField.AllCells[i, j].Figure.GetCopy());
                }
            }
        }
        public Figure GetFigureToFill((int x, int y) cord)
        {
            Player firstPlayer =
                _players.Find(x => x.Side == PlayerSide.Down);
            Player secondPlayer =
                _players.Find(x => x.Side == PlayerSide.Up);


            if (_secondPlayerPawnCords.Contains(cord)) return new Pawn(secondPlayer.Color, false, _tempFigureId, cord, secondPlayer.Side);
            if (_firstPlayerPawnCords.Contains(cord)) return new Pawn(firstPlayer.Color, false, _tempFigureId, cord, firstPlayer.Side);

            if (_secondPlayerRookCords.Contains(cord)) return new Rook(secondPlayer.Color, false, _tempFigureId, cord, secondPlayer.Side);
            if (_firstPlayerRookCords.Contains(cord)) return new Rook(firstPlayer.Color, false, _tempFigureId, cord, firstPlayer.Side);

            if (_secondPlayerHorseCords.Contains(cord)) return new Horse(secondPlayer.Color, _tempFigureId, cord, secondPlayer.Side);
            if (_firstPlayerHorseCords.Contains(cord)) return new Horse(firstPlayer.Color, _tempFigureId, cord, firstPlayer.Side);

            if (_secondPlayerBishopCords.Contains(cord)) return new Bishop(secondPlayer.Color, _tempFigureId, cord, secondPlayer.Side);
            if (_firstPlayerBishopCords.Contains(cord)) return new Bishop(firstPlayer.Color, _tempFigureId, cord, firstPlayer.Side);

            if (_secondPlayerQueenCord == cord) return new Queen(secondPlayer.Color, _tempFigureId, cord, secondPlayer.Side);
            if (_firstPlayerQueenCord == cord) return new Queen(firstPlayer.Color, _tempFigureId, cord, firstPlayer.Side);

            if (_secondPlayerKingCord == cord) return new King(secondPlayer.Color, false, _tempFigureId, cord, secondPlayer.Side);
            if (_firstPlayerKingCord == cord) return new King(firstPlayer.Color, false, _tempFigureId, cord, firstPlayer.Side);

            return null;

/*            //var - we dont now which type we want to compare
            //_ - we dont care which type value has
            //when - key word in comparation which allows to show additional conditions in comporation 
            switch (cord)
            {
                case var _ when _secondPlayerPawnCords.Contains(cord):
                    break;
                default:
                    break;
            }
*/



            //IDK WHY IT DOESNT WORK
            /*            return _secondPlayerPawnCords.Contains(cord) ? new Pawn(secondPlayer.Color, false, _tempFigureId, cord) :
                               _firstPlayerPawnCords.Contains(cord) ? new Pawn(firstPlayer.Color, false, _tempFigureId, cord) :

                               _secondPlayerRookCords.Contains(cord) ? new Rook(secondPlayer.Color, false, _tempFigureId, cord) :
                               _firstPlayerRookCords.Contains(cord) ? new Rook(firstPlayer.Color, false, _tempFigureId, cord) :

                               _secondPlayerHorseCords.Contains(cord) ? new Horse(secondPlayer.Color, _tempFigureId, cord) :
                               _firstPlayerHorseCords.Contains(cord) ? new Horse(firstPlayer.Color, _tempFigureId, cord) :

                               _secondPlayerBishopCords.Contains(cord) ? new Bishop(secondPlayer.Color, _tempFigureId, cord) :
                               _firstPlayerBishopCords.Contains(cord) ? new Bishop(firstPlayer.Color, _tempFigureId, cord) :

                               _secondPlayerQueenCord == cord ? new Queen(secondPlayer.Color, _tempFigureId, cord) :
                               _firstPlayerQueenCord == cord ? new Queen(firstPlayer.Color, _tempFigureId, cord) :

                               _secondPlayerKingCord == cord ? new King(secondPlayer.Color, false, _tempFigureId, cord) :
                               _firstPlayerKingCord == cord ? new King(firstPlayer.Color, false, _tempFigureId, cord) : null;*/

        }
        public bool IfChipIsOutOfRange((int, int) cord)
        {
            return (AllCells.GetLength(0) <= cord.Item1 || 0 > cord.Item1 ||
                AllCells.GetLength(1) <= cord.Item2 || 0 > cord.Item2);
        }
        public int GetAmountOfCells()
        {
            return AllCells.Length;
        }
        public bool IfChipIsEnemys(Player player, (int, int) cord)
        {
            return (AllCells[cord.Item1, cord.Item2].Figure != null &&
                AllCells[cord.Item1, cord.Item2].Figure.FigureColor != player.Color);
        }
        public bool IfChipIsPlayers(Player player, (int, int) cord)
        {
            return (AllCells[cord.Item1, cord.Item2].Figure != null &&
                AllCells[cord.Item1, cord.Item2].Figure.FigureColor == player.Color);
        }
        public (int, int) GetDirectionBetweenTwoFig((int, int) fromFig, (int, int) toFig)
        {
            return ((fromFig.Item1 > toFig.Item1 ? _strightBackDirection :
                fromFig.Item1 < toFig.Item1 ? _straightForwarDirection :
                _standOnPlaceDirection),
                (fromFig.Item2 > toFig.Item2 ? _strightBackDirection :
                fromFig.Item2 < toFig.Item2 ? _straightForwarDirection :
                _standOnPlaceDirection));
        }
        public (int, int) GetOpositeDirection((int, int) startDir)
        {
            return ((startDir.Item1 == 1 ? _strightBackDirection :
            startDir.Item1 == -1 ? _straightForwarDirection :
            _standOnPlaceDirection),
            (startDir.Item2 == 1 ? _strightBackDirection :
            startDir.Item2 == -1 ? _straightForwarDirection :
            _standOnPlaceDirection));

        }
        public void ReassignMove(Move move)
        {
            Figure toReassign = null;
            for (int i = 0; i < move.OneMove.Count; i++)
            {
                if (i % 2 == 0)
                {
                    toReassign = AllCells[move.OneMove[i].Item1, move.OneMove[i].Item2].Figure.GetCopy();
                    AllCells[move.OneMove[i].Item1, move.OneMove[i].Item2].Figure = null;
                }
                else
                {
                    AllCells[move.OneMove[i].Item1, move.OneMove[i].Item2].Figure = toReassign.GetCopy();
                }
            }

            if (move.ConvertFigure != new ConvertPawn())
            {
                (int, int) lastCord = move.OneMove.Last();
                (int, int) figCord = AllCells[lastCord.Item1, lastCord.Item2].Figure.FigureCord;
                PlayerSide owner = AllCells[lastCord.Item1, lastCord.Item2].Figure.OwnerSide;
                if (move.ConvertFigure == ConvertPawn.Queen)
                {
                    AllCells[lastCord.Item1, lastCord.Item2].Figure = new Queen(AllCells[lastCord.Item1,
                        lastCord.Item2].Figure.FigureColor,
                        AllCells[lastCord.Item1, lastCord.Item2].Figure.FigureID, figCord, owner);
                }
                else if (move.ConvertFigure == ConvertPawn.Rook)
                {
                    AllCells[lastCord.Item1, lastCord.Item2].Figure = new Rook(AllCells[lastCord.Item1,
                        lastCord.Item2].Figure.FigureColor, true,
                        AllCells[lastCord.Item1, lastCord.Item2].Figure.FigureID, figCord, owner);
                }
                else if (move.ConvertFigure == ConvertPawn.Horse)
                {
                    AllCells[lastCord.Item1, lastCord.Item2].Figure = new Horse(AllCells[lastCord.Item1,
                        lastCord.Item2].Figure.FigureColor,
                        AllCells[lastCord.Item1, lastCord.Item2].Figure.FigureID, figCord, owner);
                }
                else if (move.ConvertFigure == ConvertPawn.Bishop)
                {
                    AllCells[lastCord.Item1, lastCord.Item2].Figure = new Bishop(AllCells[lastCord.Item1,
                        lastCord.Item2].Figure.FigureColor,
                        AllCells[lastCord.Item1, lastCord.Item2].Figure.FigureID, figCord, owner);
                }
            }
        }
        public bool IfKingCanBeHit(Player player, (int, int) cord)
        {
            //Forward
            //Cross
            //Horse
            //Pawn 
            //King

            Player enemy = GetAnoutherPlayer(player);

            for (int i = 0; i < _crossDirection.Count; i++)
            {
                if (IfKingCanBeHitInDirection(_crossDirection[i], cord, player))
                {
                    return true;
                }
            }
            for (int i = 0; i < _forwardDirection.Count; i++)
            {
                if (IfKingCanBeHitInDirection(_forwardDirection[i], cord, player))
                {
                    return true;
                }
            }
            for (int i = 0; i < _horseDirectinos.Count; i++)
            {
                (int, int) checkCord = (cord.Item1 + _horseDirectinos[i].Item1, cord.Item2 + _horseDirectinos[i].Item2);
                if (!IfChipIsOutOfRange(checkCord) &&
                    AllCells[checkCord.Item1, checkCord.Item2].Figure is Horse &&
                    AllCells[checkCord.Item1, checkCord.Item2].Figure.FigureColor == enemy.Color)
                {
                    return true;
                }
            }

            return (IfFigureCanBeHitByPawn(player, cord) ||
                IfFigCanBEHitByKing(player, cord));
        }
        public Player GetAnoutherPlayer(Player player)
        {
            return _players.Find(x => x.Login != player.Login);// _players[0] == player ? _players[1] : _players[0];
        }
        public bool IfKingCanBeHitInDirection((int, int) direction, (int, int) figCord, Player player)
        {
            int check = 0;
            do
            {
/*                if(check >= 50)
                {
                    return false;
                }*/
                figCord = (figCord.Item1 + direction.Item1, figCord.Item2 + direction.Item2);

                if (IfChipIsOutOfRange(figCord))
                {
                    return false;
                }
                else if (AllCells[figCord.Item1, figCord.Item2].Figure != null)
                {
                    if (AllCells[figCord.Item1, figCord.Item2].Figure.FigureColor != player.Color &&
                        (AllCells[figCord.Item1, figCord.Item2].Figure is Queen ||
                       (_crossDirection.Contains(direction) && AllCells[figCord.Item1, figCord.Item2].Figure is Bishop) ||
                       (_forwardDirection.Contains(direction) && AllCells[figCord.Item1, figCord.Item2].Figure is Rook)))
                    {
                        return true;
                    }
                    return false;
                }

                check++;
            } while (true);
        }
        public bool IfFigureCanBeHitByPawn(Player player, (int, int) figCord)
        {
            List<(int, int)> checkDirsCords = player.Side == PlayerSide.Down ? _pawnHitDirectionForDownSidePlayer :
                _pawnHitDirectionForUpSidePlayer;
            for (int i = 0; i < checkDirsCords.Count; i++)
            {
                (int, int) checkCord = (figCord.Item1 + checkDirsCords[i].Item1, figCord.Item2 + checkDirsCords[i].Item2);

                if (!IfChipIsOutOfRange(checkCord) && AllCells[checkCord.Item1, checkCord.Item2].Figure is Pawn &&
                    AllCells[checkCord.Item1, checkCord.Item2].Figure.FigureColor != player.Color)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IfFigCanBEHitByKing(Player player, (int, int) figCord)
        {
            return _kingHitMoveDirections.Any(x =>
            !IfChipIsOutOfRange((figCord.Item1 + x.Item1, figCord.Item2 + x.Item2)) &&
            AllCells[figCord.Item1 + x.Item1, figCord.Item2 + x.Item2].Figure is King &&
            AllCells[figCord.Item1 + x.Item1, figCord.Item2 + x.Item2].Figure.FigureColor != player.Color) ? true : false;

        }
        public (int, int) GetKingsCord(Player player)
        {
            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    if (IfChipIsPlayers(player, (i, j)) &&
                        AllCells[i, j].Figure.GetType() == typeof(King))
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }
        public AllMoves GetMovesForFigure((int x, int y) cord, Player tempPlayer)
        {
            _rays.GetKingsRayses(this, tempPlayer);

            AllMoves moves = AllCells[cord.x, cord.y].Figure.GetMoves(tempPlayer, cord, this, _rays);

            for (int i = 0; i < moves.PossibleMoves.Count; i++)
            {
                if (moves.PossibleMoves[i].HitFigure is null)
                {
                    (int, int) toStepOn = moves.PossibleMoves[i].OneMove.Last();
                    moves.PossibleMoves[i].HitFigure = AllCells[toStepOn.Item1, toStepOn.Item2].Figure;
                }
            }

            _rays = new KingRays();

            return moves;
        }
        public bool IfKingWillBeHitAfterMove(Field field, Move move, Player player)
        {
            if (move.OneMove.Last().Item1 < 0 || move.OneMove.Last().Item2 < 0)
            {
                return true;
            }

            Field tempField = new Field(field);
            tempField.ReassignMove(move);
            return tempField.IfKingCanBeHit(player, move.OneMove.Last());
        }
        public AllMoves GetMovesForCastling(Field field,
           Player player, (int, int) kingCord)
        {
            List<List<(int, int)>> castlingWays = new List<List<(int, int)>>();

            for (int i = 0; i < _castlingDirections.Count; i++)
            {
                (int, int) temp = kingCord;
                List<(int, int)> tempCaslingWay = new List<(int, int)>() { kingCord };
                int check = 0;
                do
                {
/*                    if(check >= 50)
                    {
                        break;
                    }*/
                    temp = (temp.Item1 + _castlingDirections[i].Item1, temp.Item2 + _castlingDirections[i].Item2);
                    tempCaslingWay.Add(temp);

                    if (IfChipIsOutOfRange(temp))
                    {
                        break;
                    }
                    else if (field.AllCells[temp.Item1, temp.Item2].Figure != null)
                    {
                        if (field.AllCells[temp.Item1, temp.Item2].Figure.FigureColor == player.Color &&
                            field.AllCells[temp.Item1, temp.Item2].Figure is Rook &&
                            !((Rook)field.AllCells[temp.Item1, temp.Item2].Figure).IsFirstMoveMaken)
                        {
                            castlingWays.Add(tempCaslingWay);
                        }
                        break;
                    }
                    check++;
                } while (true);
            }

            AllMoves res = new AllMoves();

            for (int i = 0; i < castlingWays.Count; i++)
            {
                bool check = true;

                for (int j = 0; j < castlingWays[i].Count; j++)
                {
                    if (field.IfKingCanBeHit(player, castlingWays[i][j]))
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    List<(int, int)> temp = new List<(int, int)>();
                    if (castlingWays[i].Count == _shortCastling)
                    {
                        temp.Add(castlingWays[i][_firstInShortCastling]);
                        temp.Add(castlingWays[i][_secondInShortCastling]);
                        temp.Add(castlingWays[i][_thirdInShortCastling]);
                        temp.Add(castlingWays[i][_forthInShortCastling]);
                        res.PossibleMoves.Add(new Move(temp));
                    }
                    else if (castlingWays[i].Count == _longCastling)
                    {
                        temp.Add(castlingWays[i][_firstInLongCastling]);
                        temp.Add(castlingWays[i][_secondInLongCastling]);
                        temp.Add(castlingWays[i][_thirdInLongCastling]);
                        temp.Add(castlingWays[i][_forthInShortCastling]);
                        res.PossibleMoves.Add(new Move(temp));
                    }
                }
            }

            return res;
        }
        public bool CheckForCheckMate(Field field, Player player)
        {
            if (field.GetKingsCord(GetAnoutherPlayer(player)) == (-1, -1))
            {
                return false;
            }
            return field.GetAllMoves(GetAnoutherPlayer(player)).PossibleMoves.Count == 0 &&
                IfKingCanBeHit(GetAnoutherPlayer(player), GetKingsCord(GetAnoutherPlayer(player)));
        }
        public AllMoves GetAllMoves(Player player)
        {
            AllMoves moves = new AllMoves();

            KingRays rays = new KingRays();
            rays.GetKingsRayses(this, player);

            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    if (IfChipIsPlayers(player, (i, j)))
                    {
                        moves.PossibleMoves.AddRange(AllCells[i, j].Figure.GetMoves(
                                player, (i, j), this, rays).PossibleMoves);
                    }
                }
            }
            return moves;
        }
        public void IfPawnWentToTheEndOfField(Move move)
        {
            if (move.OneMove.Count != 2)
            {
                return;//Move is castling;
            }
            (int, int) lastCord = move.OneMove.Last();
            if ((lastCord.Item1 == 0 || lastCord.Item1 == AllCells.GetLength(0) - 1) &&
                AllCells[lastCord.Item1, lastCord.Item2].Figure is Pawn)
            {
                _ifNeedToConvertFig = true;
            }
        }
        public bool IfNeedToConvertFigure()
        {
            return _ifNeedToConvertFig;
        }
        public void ClearConvertationVariable()
        {
            _ifNeedToConvertFig = false;
        }
        public (int, int) GetFigureCordToConvert()
        {
            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                if (i == 0 || i == AllCells.GetLength(0) - 1)
                {
                    for (int j = 0; j < AllCells.GetLength(1); j++)
                    {
                        if (!(AllCells[i, j].Figure is null) && AllCells[i, j].Figure is Pawn)
                        {
                            return (i, j);
                        }
                    }
                }
            }
            return (-1, -1);
        }
        public void ConvertFigure((int x, int y) cord, ConvertPawn type)
        {
            Figure figure = AllCells[cord.x, cord.y].Figure;
            AllCells[cord.x, cord.y].Figure = GetConvertedFigure(type, figure);
        }
        public Figure GetConvertedFigure(ConvertPawn type, Figure figure)
        {
            if (type == ConvertPawn.Queen) return new Queen(figure.FigureColor, figure.FigureID, figure.FigureCord, figure.OwnerSide);
            if (type == ConvertPawn.Bishop) return new Bishop(figure.FigureColor, figure.FigureID, figure.FigureCord, figure.OwnerSide);
            if (type == ConvertPawn.Rook) return new Rook(figure.FigureColor, true, figure.FigureID, figure.FigureCord, figure.OwnerSide);
            if (type == ConvertPawn.Horse) return new Horse(figure.FigureColor, figure.FigureID, figure.FigureCord, figure.OwnerSide);
            return null;

            /*            return type == ConvertPawn.Queen ? new Queen(figure.FigureColor, figure.FigureID, figure.FigureCord, figure.OwnerSide) :
                            type == ConvertPawn.Bishop ? new Bishop(figure.FigureColor, figure.FigureID, figure.FigureCord, figure.OwnerSide) :
                            type == ConvertPawn.Rook ? new Rook(figure.FigureColor, true, figure.FigureID, figure.FigureCord, figure.OwnerSide) :
                            type == ConvertPawn.Queen ? new Queen(figure.FigureColor, figure.FigureID, figure.FigureCord, figure.OwnerSide) : null;*/
        }
        public void IfSpecialChipIsMoved(Move move)
        {
            for (int i = 0; i < move.OneMove.Count; i++)
            {
                if (i % 2 != 0 && i != 0)
                {
                    (int, int) cord = move.OneMove[i];
                    if (AllCells[cord.Item1, cord.Item2].Figure is Pawn)
                    {
                        ((Pawn)AllCells[cord.Item1, cord.Item2].Figure).IsFirstMoveMaken = true;
                    }
                    else if (AllCells[cord.Item1, cord.Item2].Figure is Rook)
                    {
                        ((Rook)AllCells[cord.Item1, cord.Item2].Figure).IsFirstMoveMaken = true;
                    }
                    else if (AllCells[cord.Item1, cord.Item2].Figure is King)
                    {
                        ((King)AllCells[cord.Item1, cord.Item2].Figure).IsFirstMoveMaken = true;
                    }
                }
            }
        }
        public bool IfEnemyCanMakeMoves(Player player)
        {
            return GetAllMoves(GetAnoutherPlayer(player)).PossibleMoves.Count == 0;
        }
        public void DeclineMove(Move move)
        {
            //If its castling move
            if (move.OneMove.Count > 2)
            {
                (int, int) kingStartPos = move.OneMove[0];
                (int, int) rookStartPos = move.OneMove[2];
                Move backMove = new Move(new List<(int, int)> { move.OneMove[1],
                    move.OneMove[0], move.OneMove[3], move.OneMove[2] });
                ReassignMove(backMove);

                ((King)AllCells[kingStartPos.Item1, kingStartPos.Item2].Figure).IsFirstMoveMaken = false;
                ((Rook)AllCells[rookStartPos.Item1, rookStartPos.Item2].Figure).IsFirstMoveMaken = false;

                return;
            }
            /////
            (int, int) from = move.OneMove.First();
            (int, int) to = move.OneMove.Last();
            
            //if figure was converted
            if(!(move.ConvertFigure is null))
            {
                Figure convertFig = AllCells[to.Item1, to.Item2].Figure;
                AllCells[to.Item1, to.Item2].Figure = new Pawn(convertFig.FigureColor,
                    true, convertFig.FigureID, convertFig.FigureCord, convertFig.OwnerSide);
            }

            //move figure back
            //Figure fig = AllCells[to.Item1, to.Item2].Figure;
            AllCells[from.Item1, from.Item2].Figure = _movedFigsInHistory[_movedFigsInHistory.Count - 1];
            AllCells[to.Item1, to.Item2].Figure = null;

            //if figure hit another figure
            if(!(move.HitFigure is null))
            {
                AllCells[to.Item1, to.Item2].Figure = move.HitFigure.GetCopy();
            }
        }
        
        /// <summary>
        /// Adding figure before reassigning move in logic 
        /// to save all firstMovesMaken
        /// </summary>
        /// <param name="move"></param>
        public void AddMovedFigure(Move move)
        {
            (int, int) figToMove = move.OneMove.First();
            _movedFigsInHistory.Add(AllCells[figToMove.Item1, figToMove.Item2].Figure.GetCopy());
        }
        
        public void DeleteMovedFigure()
        {
            _movedFigsInHistory.RemoveAt(_movedFigsInHistory.Count - 1);
        }
        public void AddMoveInHistory(Move move)
        {
            _movesHistory.Add(move);
        }
        public Move GetLastMove()
        {
            return _movesHistory.Count > 0 ? _movesHistory.Last() : null;
        }
        public void DeleteLastMoveInHistory()
        {
            _movesHistory.RemoveAt(_movesHistory.Count - 1);
        }
        public bool IfMoveCanBeDeclined()
        {
            return _movedFigsInHistory.Count != 0;
        }
        public List<Move> GetMoveHistory()
        {
            return _movesHistory;
        }
        public void InitMovesHistory(List<Move> moves)
        {
            _movesHistory = moves;
        }

        public AllMoves GetSortedMoves(Player player)
        {
            AllMoves res = new AllMoves();
            AllMoves allMoves = GetAllMoves(player);

            AllMoves queenMoves = new AllMoves();
            AllMoves hitMoves = new AllMoves();
            AllMoves pawnMoves = new AllMoves();
            AllMoves figuresInCenter = new AllMoves();
            AllMoves elseMoves = new AllMoves();

            for (int i = 0; i < allMoves.PossibleMoves.Count; i++)
            {
                (int, int) lastCord = allMoves.PossibleMoves[i].OneMove.Last();
                (int, int) firstCord = allMoves.PossibleMoves[i].OneMove.First();

                if (AllCells[lastCord.Item1, lastCord.Item2].Figure != null)
                {
                    hitMoves.PossibleMoves.Add(allMoves.PossibleMoves[i]);
                }
                else if (_centralCell.Contains(lastCord))
                {
                    figuresInCenter.PossibleMoves.Add(allMoves.PossibleMoves[i]);
                }
                else if (AllCells[firstCord.Item1, firstCord.Item2].Figure is Pawn)
                {
                    pawnMoves.PossibleMoves.Add(allMoves.PossibleMoves[i]);
                }
                else if (AllCells[firstCord.Item1, firstCord.Item2].Figure is Queen)
                {
                    queenMoves.PossibleMoves.Add(allMoves.PossibleMoves[i]);
                }
                else
                {
                    elseMoves.PossibleMoves.Add(allMoves.PossibleMoves[i]);
                }
            }

            res.PossibleMoves.AddRange(hitMoves.PossibleMoves);
            res.PossibleMoves.AddRange(figuresInCenter.PossibleMoves);
            res.PossibleMoves.AddRange(queenMoves.PossibleMoves);
            res.PossibleMoves.AddRange(pawnMoves.PossibleMoves);
            res.PossibleMoves.AddRange(elseMoves.PossibleMoves);

            return res;
        }
        public double GetScoreForExodusVersionTwo(Player tempPlayer,
        Player startPlayer, int amountOfMoves)
        {
            if (amountOfMoves == 0)
            {
                if (tempPlayer == startPlayer)
                {
                    (int, int) kingCord = GetKingsCord(startPlayer);
                    if (IfKingCanBeHit(startPlayer, kingCord))
                    {
                        return _valueForLostMate;
                    }
                    return GetMaterialScoreForFigures(startPlayer);
                }
                else
                {
                    (int, int) kingCord = GetKingsCord(tempPlayer);
                    if (IfKingCanBeHit(tempPlayer, kingCord))
                    {
                        return _valueForWonMate;
                    }
                    return GetMaterialScoreForFigures(tempPlayer);
                }
            }
            return (GetScoreForField(startPlayer) + (startPlayer == tempPlayer ? _scoreForMovedKing : -_scoreForMovedKing));
        }
        public double GetMaterialScoreForFigures(Player player)
        {
            double matScore = 0;

            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    if (AllCells[i, j].Figure != null)
                    {
                        if (AllCells[i, j].Figure.FigureColor == player.Color)
                        {
                            matScore += AllCells[i, j].Figure.GetScoreForFigure();
                        }
                        else
                        {
                            matScore -= AllCells[i, j].Figure.GetScoreForFigure();
                        }
                    }
                }
            }
            return matScore;
        }
        public double GetScoreForField(Player startPlayer)
        {
            double resScore = 0;
            Player enemy = GetAnoutherPlayer(startPlayer);
            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    if (AllCells[i, j].Figure != null)
                    {
                        if (AllCells[i, j].Figure.FigureColor == startPlayer.Color)
                        {
                            resScore += AllCells[i, j].Figure.GetScoreForFigure();
                            resScore += AllCells[i, j].Figure.GetScoreForFigPositionOnBoard((i, j));

                        }
                        else
                        {
                            resScore -= AllCells[i, j].Figure.GetScoreForFigure();
                            resScore -= AllCells[i, j].Figure.GetScoreForFigPositionOnBoard((i, j));

                        }
                    }
                }
            }
            return Math.Round(resScore, 3);
        }
        public AllMoves GetUniqueHitMovesFromOtherMoves(
            AllMoves tempAllMoves, AllMoves toCompare)
        {
            AllMoves res = new AllMoves();

            if (toCompare is null) toCompare = new AllMoves();

            for (int i = 0; i < tempAllMoves.PossibleMoves.Count; i++)
            {
                (int, int) toStepOn = tempAllMoves.PossibleMoves[i].OneMove.Last();
                bool isUnique = false;
                if (AllCells[toStepOn.Item1, toStepOn.Item2].Figure != null)
                {
                    for (int j = 0; j < toCompare.PossibleMoves.Count; j++)
                    {
                        if (tempAllMoves.PossibleMoves[i].Equals(toCompare.PossibleMoves[j]))
                        {
                            isUnique = true;
                            break;
                        }
                    }
                    if (!isUnique)
                    {
                        res.PossibleMoves.Add(tempAllMoves.PossibleMoves[i]);
                    }
                }
                else
                {
                    break;
                }
            }
            return res;
        }
        public AllMoves GetHitMovesVersionTwo(Player player) 
        {
            AllMoves legalMoves = new AllMoves();

            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    if (IfChipIsPlayers(player, (i, j)))
                    {
                        legalMoves.PossibleMoves.AddRange(AllCells[i, j].Figure.GetHitMoves(this, player, (i, j)).PossibleMoves);
                    }
                }
            }

            KingRays rays = new KingRays();
            rays.GetKingsRayses(this, player);

            AllMoves res = new AllMoves();

            for (int i = 0; i < legalMoves.PossibleMoves.Count; i++)
            {
                (int, int) first = legalMoves.PossibleMoves[i].OneMove.First();
                (int, int) last = legalMoves.PossibleMoves[i].OneMove.Last();

                if (rays.FigsThatHitsKing.Count == 1 &&
                    !rays.FigsThatProtectsKing.Contains(first) &&
                    rays.FigsThatHitsKing.Contains(last))
                {
                    res.PossibleMoves.Add(legalMoves.PossibleMoves[i]);
                }
                else
                {
                    for (int j = 0; j < rays.AllRays.Count; j++)
                    {
                        if (rays.AllRays[j].Contains(first) &&
                            rays.AllRays[j].Contains(last))
                        {
                            res.PossibleMoves.Add(legalMoves.PossibleMoves[i]);
                            break;
                        }
                        else if (rays.FigsThatHitsKing.Count == 0 &&
                                !rays.FigsThatProtectsKing.Contains(first) &&
                                !rays.FigsThatHitsKing.Contains(last))
                        {
                            res.PossibleMoves.Add(legalMoves.PossibleMoves[i]);
                            break;
                        }
                    }
                }

            }
            return res;
        }
        public AllMoves GetHitMoves(AllMoves moves)
        {
            AllMoves res = new AllMoves();
            res.PossibleMoves = moves.PossibleMoves.Where(u => AllCells[u.OneMove.Last().Item1,
                u.OneMove.Last().Item2].Figure != null).ToList();
            return res;
        }
        public int GetMoveIndexForReplay()
        {
            return _moveIndexForReplay;
        }
        public void NextMoveForReplay()
        {
            _moveIndexForReplay++;
        }
        public void PreviousMoveForReplay()
        {
            _moveIndexForReplay--;
        }
        public bool IfCanGetNextMove()
        {
            return _moveIndexForReplay + 1 < _movesHistory.Count;
        }
        public bool IfCanGetPreviousMove()
        {
            return _moveIndexForReplay >= 0;
        }
        public Move GetMoveForReplay()
        {
            return _movesHistory[_moveIndexForReplay];
        }
        public Move GetMoveFromHistory(int index)
        {
            return _movesHistory[index];
        }
        public void AddMovesFigureInReplayMode(Move move, int moveIndex)
        {
            if (_movedFigsInHistory.Count <= moveIndex)
            {

                (int, int) figToMove = move.OneMove.First();
                _movedFigsInHistory.Add(AllCells[figToMove.Item1, figToMove.Item2].Figure.GetCopy());
            }
        }

        public bool IfItsDrawByThreeEqualMoves()
        {
            if (_movedFigsInHistory.Count < _moveToTakeToCheckForEqaulDraw) return false;

            List<Move> moves = new List<Move>(); 
            for(int i = _movesHistory.Count - 1; i >= 0; i--)
            {
                if (i == _movesHistory.Count - 1 - _moveToTakeToCheckForEqaulDraw)
                {
                    break;
                }
                moves.Add(_movesHistory[i]);
            }

            //take last 6 moves
            //check by % 2 if first(or zero are equal)
            Move first = null;
            Move second = null;

            int firstMoveCounter = 0;
            int secondMoveCounter = 0;

            for(int i = 0; i < moves.Count; i++)
            {
                if (i == 0)
                {
                    first = moves[i];
                }
                else if (i == 1)
                {
                    second = moves[i];
                }
                if (first.IfMovesAreEqualByHistory(moves[i]))
                {
                    firstMoveCounter++;
                }
                else if (second.IfMovesAreEqualByHistory(moves[i])) 
                {
                    secondMoveCounter++;
                }
            }
            return firstMoveCounter >= _moveToDrawByEqualMoves || 
                secondMoveCounter >= _moveToDrawByEqualMoves;
        }
        public bool IfItsDrawByMovesWithoutHit()
        {
            if (_movedFigsInHistory.Count <= _movesWithoutHotCounter) return false;
            int moveCounter = 0;
            for(int i = _movesHistory.Count - 1; i >= 0; i--)
            {
                if (_movesHistory[i].HitFigure != null)
                {
                    return false;
                }
                if(moveCounter == _movesWithoutHotCounter)
                {
                    return true;
                }
                moveCounter++;
            }
            return false;
        }
        public bool IfSteppersFigIsInUnit((int,int) cord, Player player)
        {
            return AllCells[cord.Item1, cord.Item2].Figure != null &&
                AllCells[cord.Item1, cord.Item2].Figure.FigureColor == player.Color;
        }

        public bool IfFigIsEnemys((int,int) cord, Player player)
        {
            if (AllCells[cord.Item1, cord.Item2].Figure is null) return true;

            return AllCells[cord.Item1, cord.Item2].Figure.FigureColor != player.Color;
        }

    }
}
