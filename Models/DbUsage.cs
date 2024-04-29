using System;
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

namespace ChessDiploma.Models
{
    public static class DbUsage
    {
        private static readonly string _connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ChessDiploma;Integrated Security=True;";

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
                        if (colName != "Id") newUser[colName] = reader[i];
                    }
                    users.Add(newUser);
                }

                connection.Close();
            }
            return users;
        }

        public static void InsertGame(Game game)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO [Games]([FirstPlayer], [SecondPlayer], [StartTime]," +
                    "[EndTime], [Result]) " +
                    "VALUES(@firstPlayerId, @secondPlayerId, @startTime, @endTime, @resultId)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@firstPlayerId", GetPlayerId(game.Players[0].Login));
                command.Parameters.AddWithValue("@secondPlayerId", GetPlayerId(game.Players[1].Login));
                command.Parameters.AddWithValue("@startTime", game.StartTime);
                command.Parameters.AddWithValue("@endTime", game.EndTime);
                command.Parameters.AddWithValue("@resultId", GetGameExodusId(game.GameExodus));
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

                    Player firstPlayer = null;
                    Player secondPlayer = null;
                    DateTime start = new DateTime();
                    DateTime end = new DateTime();
                    GameResult exodus = new GameResult();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (colName != "Id")
                        {
                            if (colName == "FirstPlayer")
                            {
                                User player = GetPlayerById((int)reader[i]);
                                FillSideAndColorForUser(player, gameId, PlayerNumber.First);
                                firstPlayer = player;
                            }
                            else if (colName == "SecondPlayer")
                            {
                                //Check if ITSBot downLoaded!
                                User player = GetPlayerById((int)reader[i]);
                                FillSideAndColorForUser(player, gameId, PlayerNumber.Second);
                                secondPlayer = player;
                            }
                            else if (colName == "StartTime")
                            {
                                start = DateTime.Parse(reader[i].ToString());
                            }
                            else if (colName == "EndTime")
                            {
                                end = DateTime.Parse(reader[i].ToString());
                            }
                            else if (colName == "Result")
                            {
                                exodus = GetGameResult((int)reader[i]);
                            }
                        }
                    }
                    res.Add(new Game(firstPlayer, secondPlayer, start, end, exodus));
                }
                connection.Close();
            }
            return res;
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
            for (int i = 0; i <= (int)GameResult.Closed; i++)
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
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);

                        if (number == PlayerNumber.First)
                        {
                            if (colName == "FirstPlayerColor")
                            {
                                user.Color = GetPlaeyrColor((int)reader[i]);
                            }
                            else if (colName == "FirstPlayerSide")
                            {
                                user.Side = GetPlaeyrSide((int)reader[i]);
                            }
                        }
                        else
                        {
                            if (colName == "SecondPlayerColor")
                            {
                                user.Color = GetPlaeyrColor((int)reader[i]);
                            }
                            else if (colName == "SecondPlayerSide")
                            {
                                user.Side = GetPlaeyrSide((int)reader[i]);
                            }
                        }

                    }
                }
                connection.Close();
            }
            return user;
        }

        private static PlayerColor GetPlaeyrColor(int id)
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

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (colName == "Id")
                        {
                            userId = (int)reader[i];
                        }
                        else if (colName == "Login")
                        {
                            player.Login = reader[i].ToString();
                        }
                        else if (colName == "Password")
                        {
                            player.Password = reader[i].ToString();
                        }
                        else if (colName == "Email")
                        {
                            player.Email = reader[i].ToString();
                        }
                        else if (colName == "DateBirth")
                        {
                            player.DateBirth = (DateTime)reader[i];
                        }
                    }
                }

                connection.Close();
            }
            return GetOtherUserParams(player, userId);
        }
        private static User GetOtherUserParams(User user, int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [PlayerRating] WHERE [Id] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (colName == "Raiting")
                        {
                            user.Rating = (int)reader[i];
                        }
                        else if (colName == "Wons")
                        {
                            user.Wons = (int)reader[i];
                        }
                        else if (colName == "Losts")
                        {
                            user.Losts = (int)reader[i];
                        }
                        else if (colName == "Draws")
                        {
                            user.Draws = (int)reader[i];
                        }
                    }
                }
                connection.Close();
            }
            return user;
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

                command.Parameters.AddWithValue("@moveFromX", CheckForNull(moveFromX));
                command.Parameters.AddWithValue("@moveFromY", CheckForNull(moveFromY));

                command.Parameters.AddWithValue("@moveToX", CheckForNull(moveToX));
                command.Parameters.AddWithValue("@moveToY", CheckForNull(moveToY));

                command.Parameters.AddWithValue("@hitFigType", CheckForNull(hitFigId));
                command.Parameters.AddWithValue("@convertFigType", CheckForNull(convertFigId));
                command.Parameters.AddWithValue("@playerColor", playerColorId);
                command.Parameters.AddWithValue("@timeOnTimer", timeOnTimer);
                command.Parameters.AddWithValue("@castling", CheckForNull(castlingId));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private static object CheckForNull(object smth)
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
            return null;

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
                    int id = -1;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        if (colName == "Id") id = (int)reader[i];
                        else
                        {
                            string type = reader[i].ToString();
                            if (figure is Pawn && type == "Pawn" ||
                                figure is Rook && type == "Rook" ||
                                figure is Horse && type == "Horse" ||
                                figure is Bishop && type == "Bishop" ||
                                figure is Queen && type == "Queen")
                            {
                                return id;
                            }
                        }
                    }
                }
                connection.Close();
            }
            return null;
        }
        private static char? GetYCord(int cord, Move move)
        {
            if (move.OneMove.Count > 2)
            {
                return null;
            }
            List<char> _lettersToSave = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

            return (char?)GetYCordLetterId(_lettersToSave[cord]);
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
            if (move.OneMove.Count > 2) return null;
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
                    " [Losts] = @losts, [Draws] = @draws WHERE [UserId] = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@gameAmount", (user.Wons + user.Losts + user.Draws));
                command.Parameters.AddWithValue("@wons", user.Wons);
                command.Parameters.AddWithValue("@losts", user.Losts);
                command.Parameters.AddWithValue("@draws", user.Draws);
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

            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [Move] WHERE [GameId] = @gameId";
                SqlCommand command = new SqlCommand(query, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for(int i = 0; i < reader.FieldCount; i++)
                    {

                    }
                }

                connection.Close();
            }
            return res;
        }
        private static int GetGameIdByGame(Game game)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TOP 1 [Id] FROM [Game] WHERE [FirstPlayer] = @firstPlayerId AND " +
                    "[SecondPlayer] = @secondPlayerId AND [StartTime] = @startDate AND [EndTime] = @endDate AND [Result] = @result";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@firstPlayerId", GetPlayerId(game.Players[0].Login));
                command.Parameters.AddWithValue("@secondPlayerId", GetPlayerId(game.Players[1].Login));
                command.Parameters.AddWithValue("@startDate", game.StartTime);
                command.Parameters.AddWithValue("@endDate", game.EndTime);
                command.Parameters.AddWithValue("@result", GetResultId(game.GameExodus));

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

                string query = "SELECT TOP 1 [Id] WHERE [Result] = @res";
                SqlCommand command = new SqlCommand();

                command.Parameters.AddWithValue("@res", result.ToString());

                int res = (int)command.ExecuteScalar();

                connection.Close();
                return res;
            }
        }


    }
}
