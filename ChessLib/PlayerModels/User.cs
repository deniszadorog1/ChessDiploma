using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ChessLib.Enums.Players;


namespace ChessLib.PlayerModels
{
    public class User : Player
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateBirth { get; set; } 
        public int Rating { get; set; }
        public int Wons { get; set; }
        public int Losts { get; set; }
        public int Draws { get; set; }

        public User(string name, PlayerColor playerColor, PlayerSide side, List<(string name, int amount)> hitFigures, 
            string password, string email, DateTime dateBirth, int rating, int wons, int losts, int draws) :
        base(name, playerColor, side, hitFigures)
        {
            Password = password;
            Email = email;
            DateBirth = dateBirth;
            Rating = rating;
            Wons = wons;
            Losts = losts;
            Draws = draws;
        }
        public User() : base()
        {
            Password = "";
            Email = "";
            DateBirth = new DateTime();
        }
        public User(string email, string login, string password, DateTime birth)
        {
            Login = login;
            Password = password;
            Email = email;
            DateBirth = birth;
        }

        public object this[string name]
        {
            get
            {
                return typeof(User).GetProperties().First(x => x.Name.Equals(name));
            }
            set
            {
                if(typeof(User).GetProperties().Any(x => x.Name.Equals(name)))
                {
                    PropertyInfo propertInfo = typeof(User).GetProperty(name);
                    if(propertInfo != null && propertInfo.CanWrite)
                    {
                        propertInfo.SetValue(this, value);
                    }
                    return;
                }
                throw new ArgumentException("Incorretc property name!");
            }
        }
    }
}
