﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using ChessLib.PlayerModels;
using ChessLib;
using ChessLib.Enums.Game;
using ChessLib.Enums.Players;
using ChessLib.Other;
using ChessLib.Figures;
using ChessLib.Enums.Field;
using ChessLib.Enums.Figures;
using ChessLib.Figures.Interfaces;

namespace ChessDiploma.Models
{
    public static class DbUsage
    {
        private static readonly string _connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ChessDiploma;Integrated Security=True;";

        private static readonly List<char> _lettersToSave =
            new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        private const int _amountOfDotsInUsualStep = 2;
        private struct HitFigureParams
        {
            public PlayerColor FigColor { get; set; }
            public (int, int) FigCord { get; set; }
            public PlayerSide OwnerSide { get; set; }
            public bool? IfFirstMoveMaken { get; set; }
            public int FigureId { get; set; }
        }

        public static void InsertPlayer(User player)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO [Players]([Login], [Password], [Email], [DateBirth]) " +
                    "VALUES( @login, @password, @email, @dateBirth)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", player.Login);
                command.Parameters.AddWithValue("@password", player.Password);
                command.Parameters.AddWithValue("@email", player.Email);
                command.Parameters.AddWithValue("@dateBirth", player.DateBirth);
                command.ExecuteNonQuery();


                query = "INSERT INTO [PlayerRating]([UserId], [Rating], [GameAmount], [Wons], [Losts], [Draws]) " +
                    "VALUES(@userId, @rating, @gamesAmount, @wons, @losts, @draws)";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", GetPlayerId(player.Login));
                command.Parameters.AddWithValue("@rating", player.Rating);
                command.Parameters.AddWithValue("@wons", player.Wons);
                command.Parameters.AddWithValue("@losts", player.Losts);
                command.Parameters.AddWithValue("@draws", player.Draws);
                command.Parameters.AddWithValue("@gamesAmount", (player.Wons + player.Wons + player.Draws));
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        private static int GetPlayerId(string login)
        {
            int userId = -1;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [Players] WHERE Login = @login";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", login);

                userId = (int)command.ExecuteScalar();
                connection.Close();
            }
            return userId;
        }
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [Players]";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User newUser = new User();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (colName != "Id")
                        {
                            newUser[colName] = reader[i];
                        }
                        else
                        {
                            InitResParamsFormPlayer(int.Parse(reader[i].ToString()), newUser);
                        }
                    }
                    users.Add(newUser);
                }
                connection.Close();
            }
            return users;
        }

        private static void InitResParamsFormPlayer(int id, User player)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [PlayerRating] WHERE [UserId] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    player["Draws"] = int.Parse(reader["Draws"].ToString());
                    player["Wons"] = int.Parse(reader["Wons"].ToString());
                    player["Losts"] = int.Parse(reader["Losts"].ToString());
                    player["Rating"] = int.Parse(reader["Rating"].ToString());
                }

                connection.Close();
            }
        }

        public static void InsertGame(Game game)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO [Games]([FirstPlayer], [SecondPlayer], [StartTime]," +
                    "[EndTime], [Result], [Time]) " +
                    "VALUES(@firstPlayerId, @secondPlayerId, @startTime, @endTime, @resultId, @time)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@firstPlayerId", GetPlayersIdCheckForBot(game.Players[0]));
                command.Parameters.AddWithValue("@secondPlayerId", GetPlayersIdCheckForBot(game.Players[1]));
                command.Parameters.AddWithValue("@startTime", game.StartTime);
                command.Parameters.AddWithValue("@endTime", game.EndTime);
                command.Parameters.AddWithValue("@resultId", GetGameExodusId(game.GameExodus));
                command.Parameters.AddWithValue("@time", game.GetTime());
                command.ExecuteNonQuery();

                int lastGameId = GetLastGameId();

                //add players in playerGameRaiting


                query = "INSERT INTO [PlayersGameRating]([GameId], " +
                    "[FirstPlayerRating],[FirstPlayerColor], [FirstPlayerSide]," +
                    "[SecondPlayerRating], [SecondPlayerColor], [SecondPlayerSide]) " +
                    "Values(@gameId, " +
                    "@firstPlayerRating, @firstPlayerColorId, @firstPlayerSideId," +
                    "@secondPlayerRating, @secondPlayerColorId, @secondPlayerSideId)";
                SqlCommand addPlayerGameRatingCommand = new SqlCommand(query, connection);
                addPlayerGameRatingCommand.Parameters.AddWithValue("@gameId", lastGameId);

                addPlayerGameRatingCommand.Parameters.AddWithValue("@firstPlayerRating", ((User)game.Players[0]).Rating.ToString());
                addPlayerGameRatingCommand.Parameters.AddWithValue("@firstPlayerColorId", GetPlayerColorId(game.Players[0].Color));
                addPlayerGameRatingCommand.Parameters.AddWithValue("@firstPlayerSideId", GetPlayerSideId(game.Players[0].Side));

                addPlayerGameRatingCommand.Parameters.AddWithValue("@secondPlayerRating", GetRatingForSecondPlayer(game.Players[1]));
                addPlayerGameRatingCommand.Parameters.AddWithValue("@secondPlayerColorId", GetPlayerColorId(game.Players[1].Color));
                addPlayerGameRatingCommand.Parameters.AddWithValue("@secondPlayerSideId", GetPlayerSideId(game.Players[1].Side));

                addPlayerGameRatingCommand.ExecuteNonQuery();


                connection.Close();
            }
        }

        private static object GetPlayersIdCheckForBot(Player player)
        {
            return player is Bot ? DBNull.Value : (object)GetPlayerId(player.Login);
        }

        private static int GetRatingForSecondPlayer(Player player)
        {
            return player is User ? ((User)player).Rating : 0;
        }

        private static int GetPlayerColorId(PlayerColor color)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [PlayerColor] WHERE [Type] = @color";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@color", color.ToString());

                res = (int)command.ExecuteScalar();

                connection.Close();
            }
            return res;
        }
        private static int GetPlayerSideId(PlayerSide side)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [PlayerSide] WHERE [Type] = @side";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@side", side.ToString());

                res = (int)command.ExecuteScalar();

                connection.Close();
            }
            return res;
        }

        private static int GetLastGameId()
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 [Id] FROM [Games] ORDER BY [Id] DESC";

                SqlCommand command = new SqlCommand(query, connection);
                res = (int)command.ExecuteScalar();

                connection.Close();

                return res;
            }
        }
        private static int GetGameExodusId(GameResult result)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1[Id] FROM [GameResult] WHERE [Result] = @result";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@result", result.ToString());

                res = (int)command.ExecuteScalar();
                connection.Close();
            }
            return res;
        }

        public static List<Game> GetAllGames()
        {
            List<Game> res = new List<Game>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [Games]";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                int gameId = -1;
                while (reader.Read())
                {
                    gameId = (int)reader["id"];

                    Player firstPlayer = GetPlayerById(Convert.ToInt32(reader["FirstPlayer"]));
                    FillSideAndColorForUser((User)firstPlayer, gameId, PlayerNumber.First);
                    firstPlayer = GetPlayerCheckingForType((int)reader["FirstPlayer"], null, gameId, PlayerNumber.First);


                    Player secondPlayer;
                    int? id;
                    if (reader["SecondPlayer"] == DBNull.Value) id = null;
                    else id = (int)reader["SecondPlayer"];
                    secondPlayer = GetPlayerCheckingForType(id, firstPlayer, gameId, PlayerNumber.Second);


                    DateTime start = DateTime.Parse(reader["StartTime"].ToString());
                    DateTime end = DateTime.Parse(reader["EndTime"].ToString()); ;
                    GameResult exodus = GetGameResult((int)reader["Result"]);
                    int time = reader["Time"] == DBNull.Value ? -1 : (int)reader["Time"];

                    res.Add(new Game(firstPlayer, secondPlayer, start, end, exodus));
                    res.Last().InitTime(time);
                }
                connection.Close();
            }
            return res;
        }
        private static Player GetPlayerCheckingForType(int? id, Player anPlayer, int gameId, PlayerNumber number)
        {
            if (id == null)//Bot
            {
                Bot bot = new Bot();
                bot.Login = "Bot Bob";
                bot.Color = anPlayer is null ? PlayerColor.White :
                anPlayer.Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
                bot.Side = anPlayer is null ? PlayerSide.Down :
                    anPlayer.Side == PlayerSide.Down ? PlayerSide.Up : PlayerSide.Down;
                return bot;
            }
            else//User
            {
                User player = GetPlayerById(int.Parse(id.ToString()));
                FillSideAndColorForUser(player, gameId, number);

                if (!(anPlayer is null) && anPlayer.Color == player.Color)
                {
                    player.Color = anPlayer.Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
                    player.Side = anPlayer.Side == PlayerSide.Down ? PlayerSide.Up : PlayerSide.Down;
                }
                return player;
            }
        }
        private static GameResult GetGameResult(int id)
        {
            GameResult res = new GameResult();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Result] FROM [GameResult] WHERE [Id] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                res = GetGameResult(command.ExecuteScalar().ToString());

                connection.Close();
            }
            return res;
        }
        private static GameResult GetGameResult(string gameResult)
        {
            for (int i = 0; i <= (int)GameResult.Draw; i++)
            {
                if (((GameResult)i).ToString() == gameResult)
                {
                    return (GameResult)i;
                }
            }
            return new GameResult();
        }
        private static User FillSideAndColorForUser(User user, int gameId, PlayerNumber number)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [PlayersGameRating] WHERE [GameId] = @gameId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@gameId", gameId);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    if (number == PlayerNumber.First)
                    {
                        user.Color = GetPlaeyrColorById((int)reader["FirstPlayerColor"]);
                        user.Side = GetPlaeyrSide((int)reader["FirstPlayerSide"]);
                    }
                    else
                    {

                        user.Color = GetPlaeyrColorById((int)reader["SecondPlayerColor"]);
                        user.Side = GetPlaeyrSide((int)reader["SecondPlayerSide"]);
                    }
                }
                connection.Close();
            }
            return user;
        }
        private static PlayerColor GetPlaeyrColorById(int id)
        {
            string color = "";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Type] FROM [PlayerColor] WHERE [Id] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                color = command.ExecuteScalar().ToString();

                connection.Close();
            }
            return GetPlayerColor(color);
        }
        private static PlayerColor GetPlayerColor(string color)
        {
            for (int i = 0; i <= (int)PlayerColor.Black; i++)
            {
                if (((PlayerColor)i).ToString() == color)
                {
                    return (PlayerColor)i;
                }
            }
            return new PlayerColor();
        }
        private static PlayerSide GetPlaeyrSide(int id)
        {
            string side = "";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Type] FROM [PlayerSide] WHERE [Id] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                side = command.ExecuteScalar().ToString();

                connection.Close();
            }
            return GetPlayerSide(side);
        }
        private static PlayerSide GetPlayerSide(string side)
        {
            for (int i = 0; i <= (int)PlayerSide.Down; i++)
            {
                if (((PlayerSide)i).ToString() == side)
                {
                    return (PlayerSide)i;
                }
            }
            return new PlayerSide();
        }
        private static User GetPlayerById(int id)
        {
            User player = new User();
            int userId = -1;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [Players] WHERE [Id] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    userId = (int)reader["Id"];

                    player = new User()
                    {
                        Login = reader["Login"].ToString(),
                        Password = reader["Password"].ToString(),
                        Email = reader["Email"].ToString(),
                        DateBirth = DateTime.Parse(reader["DateBirth"].ToString())
                    };
                }

                connection.Close();
            }
            GetOtherUserParams(ref player, userId);
            return player;
        }
        private static void GetOtherUserParams(ref User user, int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [PlayerRating] WHERE [Id] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user.Rating = (int)reader["Rating"];
                    user.Wons = (int)reader["Wons"];
                    user.Losts = (int)reader["Losts"];
                    user.Draws = (int)reader["Draws"];
                }
                connection.Close();
            }
        }
        public static void InsertMove(Move move)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO [Move]([GameId], " +
                    "[MoveFromX], [MoveFromY]," +
                    "[MoveToX], [MoveToY]," +
                    "[HitFigType], [ConvertFigType], " +
                    "[PlayerColor], [TimeOnTimer], [CastlingId]) " +
                    "Values(@gameId, @moveFromX, @moveFromY, @moveToX, @moveToY, " +
                    "@hitFigType, @convertFigType, @playerColor, @timeOnTimer, @castling)";

                SqlCommand command = new SqlCommand(query, connection);


                int gameId = GetLastGameId();

                int? moveFromX = GetXCord(move.OneMove[0].Item2, move);
                int? moveFromY = GetYCord(move.OneMove[0].Item1, move);

                int? moveToX = GetXCord(move.OneMove[1].Item2, move);
                int? moveToY = GetYCord(move.OneMove[1].Item1, move);

                int? hitFigId = GetHitFigureType(move.HitFigure);
                int? convertFigId = GetFigureId(move.ConvertFigure.ToString());
                int playerColorId = GetPlayerColorId(move.GetPlayerColor());
                int timeOnTimer = move.GetTimeOnTimer();
                int? castlingId = GetCastlingId(move.GetCastlingType());


                command.Parameters.AddWithValue("@gameId", gameId);

                command.Parameters.AddWithValue("@moveFromX", GetCheckedObject(moveFromX));
                command.Parameters.AddWithValue("@moveFromY", GetCheckedObject(moveFromY));

                command.Parameters.AddWithValue("@moveToX", GetCheckedObject(moveToX));
                command.Parameters.AddWithValue("@moveToY", GetCheckedObject(moveToY));

                command.Parameters.AddWithValue("@hitFigType", GetCheckedObject(hitFigId));
                command.Parameters.AddWithValue("@convertFigType", GetCheckedObject(convertFigId));
                command.Parameters.AddWithValue("@playerColor", playerColorId);
                command.Parameters.AddWithValue("@timeOnTimer", timeOnTimer);
                command.Parameters.AddWithValue("@castling", GetCheckedObject(castlingId));

                command.ExecuteNonQuery();

                if (!(hitFigId is null))
                {
                    InsertHitFigureParmas(move.HitFigure, GetLastMoveId(), gameId);
                }
                connection.Close();
            }
        }
        private static int GetLastMoveId()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [Move] ORDER BY [Id] DESC;";

                SqlCommand command = new SqlCommand(query, connection);

                int res = (int)command.ExecuteScalar();

                connection.Close();
                return res;
            }
        }
        private static object GetCheckedObject(object smth)
        {
            return smth == null ? DBNull.Value : smth;
        }
        private static int? GetCastlingId(CastlingType? type)
        {
            if (type is null) return null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1[Id] FROM [Castling] WHERE [Type] = @type";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@type", type.ToString());

                int res = (int)command.ExecuteScalar();
                connection.Close();

                return res;
            }
        }
        private static int? GetHitFigureType(Figure figure)
        {
            if (figure is null) return null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [FigureType]";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = (int)reader["Id"];
                    string type = reader["Type"].ToString();

                    if (figure.GetType().Name == type) return id;
                    //if (figure is Pawn && figure.GetType() == "Pawn" ||
                    //    figure is Rook && type == "Rook" ||
                    //    figure is Horse && type == "Horse" ||
                    //    figure is Bishop && type == "Bishop" ||
                    //    figure is Queen && type == "Queen")
                    //{
                    //    return id;
                    //}
                }
                connection.Close();
            }
            return null;
        }
        private static char? GetYCord(int cord, Move move)
        {
            return move.OneMove.Count > _amountOfDotsInUsualStep ? null :
                (char?)GetYCordLetterId(_lettersToSave[cord]);
        }
        private static int? GetYCordLetterId(char letter)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1[Id] FROM [Vertical] WHERE [Value] = @letter";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@letter", letter);

                int res = (int)command.ExecuteScalar();

                connection.Close();

                return res;
            }
        }
        private static int? GetXCord(int cord, Move move)
        {
            if (move.OneMove.Count > _amountOfDotsInUsualStep) return null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [Horizontal] WHERE [Value] = @cord";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@cord", cord);

                int res = (int)command.ExecuteScalar();

                connection.Close();
                return res;
            }
        }
        private static int? GetFigureId(string figureName)
        {
            int? res = -1;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [FigureType] WHERE [Type] = @type";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@type", figureName);

                res = (int?)command.ExecuteScalar();

                connection.Close();
            }
            return res;
        }
        public static void UpdateUsersResults(User user)
        {
            int userId = GetUserIdByLogin(user.Login);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "UPDATE [PlayerRating] SET [GameAmount] = @gameAmount, [Wons] = @wons," +
                    " [Losts] = @losts, [Draws] = @draws, [Rating] = @raiting WHERE [UserId] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@gameAmount", (user.Wons + user.Losts + user.Draws));
                command.Parameters.AddWithValue("@wons", user.Wons);
                command.Parameters.AddWithValue("@losts", user.Losts);
                command.Parameters.AddWithValue("@draws", user.Draws);
                command.Parameters.AddWithValue("@raiting", user.Rating);
                command.Parameters.AddWithValue("@id", userId);

                command.ExecuteNonQuery();

                connection.Close();
            }

        }
        private static int GetUserIdByLogin(string login)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [Players] WHERE [Login] = @login";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", login);

                int res = (int)command.ExecuteScalar();


                connection.Close();
                return res;
            }
        }
        public static List<Move> GetGameMoves(Game game)
        {
            List<Move> res = new List<Move>();
            int gameId = GetGameIdByGame(game);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [Move] WHERE [GameId] = @gameId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("gameId", gameId);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Move move = new Move();

                    int moveId = (int)reader["Id"];

                    //Get the OneMove
                    if (reader["MoveFromX"] != DBNull.Value)
                    {
                        List<(int, int)> oneMove = new List<(int, int)>();

                        oneMove.Add(GetPointOnOneMove((int)reader["MoveFromY"], (int)reader["MoveFromX"]));
                        oneMove.Add(GetPointOnOneMove((int)reader["MoveToY"], (int)reader["MoveToX"]));

                        move.OneMove = oneMove;
                    }

                    //Get player Color
                    move.ChangeSteperColor(GetPlaeyrColorById((int)reader["PlayerColor"]));


                    //Get hit type
                    if (reader["HitFigType"] != DBNull.Value)
                    {
                        move.HitFigure = GetHitFigure((int)reader["HitFigType"], moveId, gameId);
                    }

                    //Get convert type 
                    if (reader["ConvertFigType"] != DBNull.Value)
                    {
                        move.ConvertFigure = GetFigureTypeById((int)reader["ConvertFigType"]);
                    }

                    //Get castling type
                    if (reader["CastlingId"] != DBNull.Value)
                    {
                        move.InitCastling(GetCastlingType((int)reader["CastlingId"]));
                        move.InitMoveInCastling((CastlingType)move.GetCastlingType(),
                            GetPlayerSide(game.Players, move.GetPlayerColor()));
                    }

                    //GetTime on timer
                    move.AssignTime(reader["TimeOnTimer"] == DBNull.Value ? -1 : (int)reader["TimeOnTimer"]);

                    res.Add(move);
                }
                connection.Close();
            }
            return res;
        }

        private static PlayerSide GetPlayerSide(List<Player> players, PlayerColor color)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Color == color)
                {
                    return players[i].Side;
                }
            }
            return new PlayerSide();
        }
        private static Figure GetHitFigure(int id, int moveId, int gameId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Type] FROM [FigureType] WHERE [Id] = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                string res = command.ExecuteScalar().ToString();

                connection.Close();
                return GetFigureByString(GetTypeByString(res), moveId, gameId);
            }
        }
        private static FigType GetTypeByString(string type)
        {
            return type == "Pawn" ? FigType.Pawn :
                   type == "Rook" ? FigType.Rook :
                   type == "Horse" ? FigType.Horse :
                   type == "Bishop" ? FigType.Bishop :
                   type == "Queen" ? FigType.Queen : 
                   type == "King" ? FigType.King : new FigType();
        }
        private static Figure GetFigureByString(FigType figType, int moveId, int gameId)
        {
            HitFigureParams hitFigParams = GetHitFigureParams(moveId, gameId);

            if (figType == FigType.Pawn) 
            {
                return new Pawn(hitFigParams.FigColor, (bool)hitFigParams.IfFirstMoveMaken,
                    hitFigParams.FigureId, hitFigParams.FigCord, hitFigParams.OwnerSide);
            }
            else if (figType == FigType.Rook)
            {
                return new Rook(hitFigParams.FigColor, (bool)hitFigParams.IfFirstMoveMaken,
                    hitFigParams.FigureId, hitFigParams.FigCord, hitFigParams.OwnerSide);
            }
            else if (figType == FigType.Horse)
            {
                return new Horse(hitFigParams.FigColor, hitFigParams.FigureId,
                    hitFigParams.FigCord, hitFigParams.OwnerSide);
            }
            else if (figType == FigType.Bishop)
            {
                return new Bishop(hitFigParams.FigColor, hitFigParams.FigureId,
                    hitFigParams.FigCord, hitFigParams.OwnerSide);
            }
            else if (figType == FigType.Queen)
            {
                return new Queen(hitFigParams.FigColor, hitFigParams.FigureId,
                    hitFigParams.FigCord, hitFigParams.OwnerSide);
            }
            return null;
        }
        private static ConvertPawn GetFigureTypeById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Type] FROM [FigureType] WHERE [Id] = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                string res = command.ExecuteScalar().ToString();

                connection.Close();
                return GetConvertType(res);
            }
        }
        private static ConvertPawn GetConvertType(string type)
        {
            for (int i = 0; i <= (int)ConvertPawn.Bishop; i++)
            {
                if (((ConvertPawn)i).ToString() == type)
                {
                    return (ConvertPawn)i;
                }
            }
            return new ConvertPawn();
        }
        private static CastlingType GetCastlingType(int typeId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Type] FROM [Castling] WHERE [Id] = @typeId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@typeId", typeId);

                string res = command.ExecuteScalar().ToString();

                connection.Close();
                return GetCastling(res);
            }
        }
        private static CastlingType GetCastling(string type)
        {
            for (int i = 0; i <= (int)CastlingType.Long; i++)
            {
                if (((CastlingType)i).ToString() == type)
                {
                    return (CastlingType)i;
                }
            }
            return new CastlingType();
        }
        private static (int, int) GetPointOnOneMove(int xId, int yId)
        {
            return (GetXCordById(xId), GetYCordById(yId));
        }
        private static int GetYCordById(int yId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Value] FROM [Vertical] WHERE [Id] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", yId);

                int res = _lettersToSave.FindIndex(x => x == command.ExecuteScalar().ToString()[0]);

                connection.Close();
                return res;
            }
        }
        private static int GetYIDByValue(int value)
        {
            char converted = _lettersToSave[value];
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [Vertical] WHERE [Value] = @value";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@value", converted);

                int res = (int)command.ExecuteScalar();

                connection.Close();
                return res;
            }
        }
        private static int GetXCordById(int xId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Value] FROM [Horizontal] WHERE [Id] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", xId);

                object asd = command.ExecuteScalar();

                int res = int.Parse(asd.ToString());

                connection.Close();
                return res;
            }
        }
        private static int GetXIDByValue(int value)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [Horizontal] WHERE [Value] = @value";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@value", value);

                int res = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();
                return res;
            }
        }
        private static int GetGameIdByGame(Game game)
        {
            List<Game> games = GetAllGames();

            for (int i = 0; i < games.Count; i++)
            {
                if (games[i].Equals(game))
                {
                    return (i + 1);
                }
/*                if (games[i].Players[0].Login == game.Players[0].Login &&
                    games[i].Players[1].Login == game.Players[1].Login &&
                    games[i].GameExodus == game.GameExodus &&
                    games[i].StartTime == game.StartTime &&
                    games[i].EndTime == game.EndTime) //operator overriding
                {
                    return (i + 1);
                }*/
            }
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [Games] WHERE [FirstPlayer] = @firstPlayerId AND " +
                    "[SecondPlayer] = @secondPlayerId AND [StartTime] = @startDate AND [EndTime] = @endDate AND [Result] = @result";

                int firstPlayerId = GetPlayerId(game.Players[0].Login);
                int secondPlayerId = GetPlayerId(game.Players[1].Login);

                int resutId = GetResultId(game.GameExodus);

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@firstPlayerId", firstPlayerId);
                command.Parameters.AddWithValue("@secondPlayerId", secondPlayerId);
                command.Parameters.AddWithValue("@startDate", game.StartTime);
                command.Parameters.AddWithValue("@endDate", game.EndTime);
                command.Parameters.AddWithValue("@result", resutId);

                int res = (int)command.ExecuteScalar();
                connection.Close();
                return res;

            }


        }
        private static int GetResultId(GameResult result)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [GameResult] WHERE [Result] = @res";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@res", result.ToString());

                int res = (int)command.ExecuteScalar();

                connection.Close();
                return res;
            }
        }
        private static HitFigureParams GetHitFigureParams(int moveId, int gameId)
        {
            HitFigureParams res = new HitFigureParams();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 * FROM [HitFigureParams] WHERE [MoveId] = @moveId AND [GameId] = @gameId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@moveId", moveId);
                command.Parameters.AddWithValue("@gameId", gameId);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    res.FigColor = GetPlaeyrColorById((int)reader["FigureColor"]);
                    res.FigCord = GetPointOnOneMove((int)reader["CordY"], (int)reader["CordX"]);
                    res.OwnerSide = GetPlaeyrSide((int)reader["OwnerSide"]);
                    res.IfFirstMoveMaken = reader["IfFirstMoveMaken"] == DBNull.Value ? null : (bool?)reader["IfFirstMoveMaken"];
                    res.FigureId = (int)reader["FigureId"];
                }

                connection.Close();
            }
            return res;
        }


        private static void InsertHitFigureParmas(Figure figure, int moveId, int gameId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO [HitFigureParams]([MoveId], [GameId], [FigureColor], [IfFirstMoveMaken], " +
                    "[CordX], [CordY], [OwnerSide], [FigureId]) " +
                    "VALUES(@moveId, @gameId, @colorId, @ifFirstMoveMaken, @cordXId, @cordYId, @ownerSideId, @figureId)";
                SqlCommand command = new SqlCommand(query, connection);

                int colorId = GetPlayerColorId(figure.FigureColor);

                int xCord = GetXIDByValue(figure.FigureCord.Item2);
                int yCord = GetYIDByValue(figure.FigureCord.Item1);

                int sideId = GetPlayerSideId(figure.OwnerSide);

                object ifFirstMoveIsMaken = DBNull.Value;
                if (figure is IFirstMove)
                {
                    ifFirstMoveIsMaken = GetCheckedObject(IfFigureMadeFirstMove((IFirstMove)figure));
                }
                command.Parameters.AddWithValue("@moveId", moveId);
                command.Parameters.AddWithValue("@gameId", gameId);
                command.Parameters.AddWithValue("@colorId", colorId);
                command.Parameters.AddWithValue("@ifFirstMoveMaken", ifFirstMoveIsMaken);
                command.Parameters.AddWithValue("@cordXId", xCord);
                command.Parameters.AddWithValue("@cordYId", yCord);
                command.Parameters.AddWithValue("@ownerSideId", sideId);
                command.Parameters.AddWithValue("@figureId", figure.FigureID);

                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        private static int? IfFigureMadeFirstMove(IFirstMove figure)
        {
            return figure.IsFirstMoveMaken ? 1 : 0;
        }
        public static void UpdateUser(string email, string login, string passwrod, User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "UPDATE [Players] SET [Login] = @login, [Password] = @password, [Email] = @email, [DateBirth] = @date " +
                    "WHERE [Login] = @compLogin AND [Password] = @compPassword AND [Email] = @compEmail";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("login", user.Login);
                command.Parameters.AddWithValue("password", user.Password);
                command.Parameters.AddWithValue("email", user.Email);
                command.Parameters.AddWithValue("date", user.DateBirth);

                command.Parameters.AddWithValue("compLogin", login);
                command.Parameters.AddWithValue("compPassword", passwrod);
                command.Parameters.AddWithValue("compEmail", email);


                command.ExecuteNonQuery();

                //string query = "UPDATE [PlayerRating] SET [GameAmount] = @gameAmount, [Wons] = @wons," +
                //    " [Losts] = @losts, [Draws] = @draws WHERE [UserId] = @id";


                connection.Close();
            }
        }
    }
}
