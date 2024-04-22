using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Field(List<Player> players)
        {
            _players = players;
            AllCells = new Cell[_fieldHeight, _fieldWidth];

            for (int i = 0; i < AllCells.GetLength(0); i++)
            {
                for (int j = 0; j < AllCells.GetLength(1); j++)
                {
                    AllCells[i, j] = new Cell();

                    AllCells[i,j].Figure = GetFigureToFill((i, j));

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
                new Player(copyField._players[0].Name ,copyField._players[0].Color ,copyField._players[0].Side),
                new Player(copyField._players[1].Name, copyField._players[1].Color, copyField._players[1].Side)
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

            //var - we dont now which type we want to compare
            //_ - we dont care which type value has
            //when - key word in comparation which allows to show additional conditions in comporation 
            switch (cord)
            {
                case var _ when _secondPlayerPawnCords.Contains(cord):
                    break;
                default:
                    break;
            }




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


                /*                AllCells[move.OneMove[i + 1].Item1, move.OneMove[i + 1].Item2].FigureInCell =
                                    AllCells[move.OneMove[i].Item1, move.OneMove[i].Item2].FigureInCell;
                                AllCells[move.OneMove[i].Item1, move.OneMove[i].Item2].FigureInCell = null;*/
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
            return _players[0] == player ? _players[1] : _players[0];
        }
        public bool IfKingCanBeHitInDirection((int, int) direction, (int, int) figCord, Player player)
        {
            do
            {
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
        public AllMoves GetMovesForFigure((int x,int y) cord, Player tempPlayer)
        {
            _rays.GetKingsRayses(this, tempPlayer);

            return AllCells[cord.x, cord.y].Figure.GetMoves(tempPlayer, cord, this, _rays);
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
                do
                {
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
                            !((Rook)field.AllCells[temp.Item1, temp.Item2].Figure).IfFirstMoveMaken)
                        {
                            castlingWays.Add(tempCaslingWay);
                        }
                        break;
                    }
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
    }
}
