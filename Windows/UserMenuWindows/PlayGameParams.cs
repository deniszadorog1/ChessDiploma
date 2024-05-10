using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ChessLib.PlayerModels;
using ChessLib.Enums.Players;
using ChessDiploma.Models;

namespace ChessDiploma.Windows.UserMenuWindows
{
    public partial class PlayGameParams : Form
    {
        private User _user;
        private List<User> _allUsers;

        private Player _enemy = null;
        private const int _spaceBetweenEnemyLogins = 25;
        private List<int> _gameTimer = new List<int>() { -1, 5, 10, 20, 30 };

        private const string _botName = "Bot bob";

        private const int _userButtonWidthError = 15;
        private const int _userButtonHeight = 50;

        private readonly List<string> _times = new List<string>()
        {
            "No timer",
            "5 minutes",
            "10 minutes",
            "20 minutes",
            "30 minutes"
        };

        private readonly List<string> _botHard = new List<string>()
        {
            "Easy",
            "Medium",
            "Hard"
        };
        public PlayGameParams(User user, List<User> allUsers)
        {
            _user = user;
            _allUsers = allUsers;
            InitializeComponent();
            FillGameTimeBox();
        }
        public void FillEnemysPanel()
        {
            EnemyPanel.Controls.Clear();
            if (UserRadio.Checked)
            {
                _enemy = null;
                FillEnemyUsersInPanel();
            }
            else if (BotRadio.Checked)
            {
                _enemy = null;
                EnemyLoginLB.Text = "Enemy is Bot";
                FillEnemyBots();
            }
            else//Nothing checked
            {
                MessageBox.Show("Enemy doesnt chosen!", "Mistake!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void FillGameTimeBox()
        {
            for (int i = 0; i < _times.Count; i++)
            {
                GameTimeBox.Items.Add(_times[i]);
            }
        }
        public void FillEnemyUsersInPanel()
        {
            Point loc = new Point(0, 0);
            for (int i = 0; i < _allUsers.Count; i++)
            {
                if (_allUsers[i].Login != _user.Login)
                {
                    Button enemyBut = new Button();
                    enemyBut.Click += NewEnemy_Click;
                    enemyBut.Text = _allUsers[i].Login;
                    enemyBut.Font = new Font("Times New Roman", 14);
                    enemyBut.Location = loc;
                    enemyBut.Size = new Size(EnemyPanel.Width - _userButtonWidthError, _userButtonHeight);
                    EnemyPanel.Controls.Add(enemyBut);
                    loc = new Point(0, loc.Y + _spaceBetweenEnemyLogins);
                }
            }
        }
        public void FillEnemyBots()
        {
            Point loc = new Point(0, 0);
            for (int i = 0; i < _botHard.Count; i++)
            {
                Button enemyBut = new Button();
                enemyBut.Click += EnemyBotHard_Click;
                enemyBut.Text = _botHard[i];
                enemyBut.Font = new Font("Times New Roman", 14);
                enemyBut.Location = loc;
                enemyBut.Size = new Size(EnemyPanel.Width - _userButtonWidthError, _userButtonHeight);
                EnemyPanel.Controls.Add(enemyBut);
                loc = new Point(0, loc.Y + _spaceBetweenEnemyLogins);

            }
        }
        private void EnemyBotHard_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < EnemyPanel.Controls.Count; i++)
            {
                if (EnemyPanel.Controls[i] is Button)
                {
                    ((Button)EnemyPanel.Controls[i]).ForeColor = Color.Black;
                }
            }
            if (sender is Button clickedButton)
            {
                clickedButton.ForeColor = Color.DarkGreen;

                for (int i = 0; i < EnemyPanel.Controls.Count; i++)
                {
                    if (EnemyPanel.Controls[i] is Button &&
                        ((Button)EnemyPanel.Controls[i]).ForeColor == Color.DarkGreen)
                    {
                        _enemy = new Bot((i + 1));
                        if(i == EnemyPanel.Controls.Count - 1)
                        {
                            _enemy = new Bot(i);
                        }
                    }
                }
            }
        }

        private void NewEnemy_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < EnemyPanel.Controls.Count; i++)
            {
                if (EnemyPanel.Controls[i] is Button)
                {
                    ((Button)EnemyPanel.Controls[i]).ForeColor = Color.Black;
                }
            }
            if (sender is Button clickedButton)
            {
                clickedButton.ForeColor = Color.DarkGreen;
                _enemy = _allUsers.Find(x => x.Login == clickedButton.Text);
                EnemyLoginLB.Text = _enemy.Login;
            }
        }
        public void RadioChecked_RadioChecked(object sender, EventArgs e)
        {
            FillEnemysPanel();
        }
        private void BackBut_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void PlayerBut_Click(object sender, EventArgs e)
        {
            if (_enemy is null || (!WhiteRadio.Checked && !BlackRadio.Checked) || GameTimeBox.SelectedIndex == -1)
            {
                MessageBox.Show("Somthing went wrong!", "Mistake!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //MessageBox.Show("Game can be started!", "Success");

            InitColorToPlayer();

            if (_enemy is Bot)
            {
                _enemy.Login = _botName;
            }
            FillEnemy();

            Hide();
            Data.InitPlayerAndTimerInGame(_user, _enemy, _gameTimer[GameTimeBox.SelectedIndex]);
            FieldFrom game = new FieldFrom();
            game.ShowDialog();
            Show();
        }
        public void FillEnemy()
        {
            _enemy.Side = _user.Side == PlayerSide.Up ? PlayerSide.Down : PlayerSide.Up;
            _enemy.Color = _user.Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        }
        public void InitColorToPlayer()
        {
            _user.Side = PlayerSide.Down;
            if (WhiteRadio.Checked)
            {
                _user.Color = PlayerColor.White;
                return;
            }
            _user.Color = PlayerColor.Black;
        }
        /*        public Player GetEnemyToPlay()
                {
                    if (UserRadio.Checked)
                    {
                        for (int i = 0; i < _allUsers.Count; i++)
                        {
                            if (_allUsers[i].Login == EnemyLoginLB.Text)
                            {
                                Player enemy = _allUsers[i];
                                enemy.Color = _user.Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
                                enemy.Side = PlayerSide.Up;
                                return enemy;
                            }
                        }
                    }
                    return null;
                }*/
    }
}
