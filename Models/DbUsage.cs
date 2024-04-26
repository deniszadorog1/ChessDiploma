using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using ChessLib.PlayerModels;

namespace ChessDiploma.Models
{
    public static class DbUsage
    {
        private static readonly string _connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ChessDiploma;Integrated Security=True;";
    
        public static void InsertPlayer(User player)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
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
                command.Parameters.AddWithValue("@userId", GetUserId(player));
                command.Parameters.AddWithValue("@rating", player.Rating);
                command.Parameters.AddWithValue("@wons", player.Wons);
                command.Parameters.AddWithValue("@losts", player.Losts);
                command.Parameters.AddWithValue("@draws", player.Draws);
                command.Parameters.AddWithValue("@gamesAmount", (player.Wons + player.Wons + player.Draws));
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        private static int GetUserId(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT Id FROM [Players] WHERE Login = @login";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", user.Login);

                int.TryParse(command.ExecuteScalar().ToString(), out int userId);

                connection.Close();
                return userId;
            }
        }
        public static User GetPlayer(string login, string password)
        {
            User res = new User();
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [Players] WHERE Login = @login AND Password = @password";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = command.ExecuteReader();
               
                while (reader.Read())
                {
                    for(int i = 0; i < reader.FieldCount; i++)
                    {
                        string colName = reader.GetName(i);
                        res[colName] = reader[i];
                    }
                }
                connection.Close();
            }
            return res;
        }

        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using(SqlConnection connection = new SqlConnection(_connectionString))
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
                        if(colName != "Id")newUser[colName] = reader[i];
                    }
                    users.Add(newUser);
                }

                connection.Close();
            }
            return users;
        }

    }
}
