using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

using ChessLib.Enums.Players;
using ChessLib.Enums.Figures;

namespace ChessLib.PlayerModels
{
    public delegate void TimerEventHandler(object sender, EventArgs e, Form form);
    public class User : Player
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateBirth { get; set; }
        public int Rating { get; set; }
        public int Wons { get; set; }
        public int Losts { get; set; }
        public int Draws { get; set; }

        public System.Timers.Timer _gameTimer = new System.Timers.Timer();
        public Label checkTimer = new Label();

        public int _currentTime = -1;
        public int startTime = -1;

        public event TimerEventHandler TimerFinished;

        private Form _fromToCloseByTimer;

        public User(string name, PlayerColor playerColor, PlayerSide side, List<(FigType name, int amount)> hitFigures,
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
            InitTimer();
        }
        public User(string email, string login, string password, DateTime birth)
        {
            Login = login;
            Password = password;
            Email = email;
            DateBirth = birth;
            InitTimer();
        }
        public void InitTimer()
        {
            _gameTimer = new System.Timers.Timer();
            _gameTimer.Interval = 1000;
            _gameTimer.Stop();
            _gameTimer.Elapsed += (sender, e) =>
            {
                _currentTime--;
                if (_currentTime > 0)
                {
                    checkTimer.Text = GetTimerInString();
                }
                else
                {
                    _gameTimer.Stop();
                    OnTimerFinished();
                    //_fromToCloseByTimer.Close();
                }
            };
        }
        protected virtual void OnTimerFinished()
        {
            TimerEventHandler handler = TimerFinished;
            handler?.Invoke(this, EventArgs.Empty, _fromToCloseByTimer);
        }
        public void InitLabel(Label lb)
        {
            checkTimer = lb;
        }
        public string GetTimerInString()
        {
            //string formattedTime = string.Format("{0:00}:{1:00}", _currentTime / 60, _currentTime % 60);
            return string.Format("{0:00}:{1:00}", _currentTime / 60, _currentTime % 60);

        }
        public object this[string name]
        {
            get
            {
                return typeof(User).GetProperties().First(x => x.Name.Equals(name));
            }
            set
            {
                if (typeof(User).GetProperties().Any(x => x.Name.Equals(name)))
                {
                    PropertyInfo propertInfo = typeof(User).GetProperty(name);
                    if (propertInfo != null && propertInfo.CanWrite)
                    {
                        propertInfo.SetValue(this, value);
                    }
                    return;
                }
                throw new ArgumentException("Incorretct property name!");
            }
        }
        public int GetCurrnetTimeOnTimer()
        {
            return _currentTime;
        }
        public void StopTimer()
        {
            _gameTimer.Stop();
        }

        public void GetFormToCloseByTimer(Form form)
        {
            _fromToCloseByTimer = form;
        }

    }
}
