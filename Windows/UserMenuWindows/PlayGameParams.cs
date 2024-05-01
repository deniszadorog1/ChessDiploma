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

        private List<string> _botHardLevels = new List<string>() { "Easy", "Normal", "Hard" };
        private const string _botName = "Bot bob";
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
                _enemy = new Bot();
            }
            else//Nothing checked
            {
                MessageBox.Show("Enemy doesnt chosen!", "Mistake!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void FillGameTimeBox()
        {
            GameTimeBox.Items.Add("No timer");
            GameTimeBox.Items.Add("5 minutes");
            GameTimeBox.Items.Add("10 minutes");
            GameTimeBox.Items.Add("20 minutes");
            GameTimeBox.Items.Add("30 minutes");
        }
        public void FillEnemyUsersInPanel()
        {
            Point loc = new Point(0, 0);
            for (int i = 0; i < _allUsers.Count; i++)
            {
                if (_allUsers[i].Login != _user.Login)
                {
                    Label newEnemy = new Label();
                    newEnemy.Text = _allUsers[i].Login;
                    newEnemy.Font = new Font("Times New Roman", 14);
                    newEnemy.Location = loc;
                    newEnemy.Click += NewEnemy_Click;
                    EnemyPanel.Controls.Add(newEnemy);
                    loc = new Point(0, loc.Y + _spaceBetweenEnemyLogins);
                }
            }
        }
        private void NewEnemy_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < EnemyPanel.Controls.Count; i++)
            {
                if (EnemyPanel.Controls[i] is Label)
                {
                    ((Label)EnemyPanel.Controls[i]).ForeColor = Color.Black;
                }
            }
            if (sender is Label clickedLabel)
            {
                clickedLabel.ForeColor = Color.DarkGreen;
                _enemy = _allUsers.Find(x => x.Login == clickedLabel.Text);
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
        public Player GetEnemyToPlay()
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
        }
    }
}
