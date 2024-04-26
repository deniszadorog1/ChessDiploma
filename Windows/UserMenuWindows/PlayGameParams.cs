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

namespace ChessDiploma.Windows.UserMenuWindows
{
    public partial class PlayGameParams : Form
    {
        private User _user;
        private List<User> _allUsers;

        private User _enemy = null;

        private const int _spaceBetweenEnemyLogins = 25;
        public PlayGameParams(User user, List<User> allUsers)
        {
            _user = user;
            _allUsers = allUsers;
            InitializeComponent();
        }
        public void FillEnemysPanel()
        {
            EnemyPanel.Controls.Clear();
            if (UserRadio.Checked)
            {
                FillEnemyUsersInPanel();
            }
            else if (BotRadio.Checked)
            {

            }
            else//Nothing checked
            {

            }
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
            for(int i = 0; i < EnemyPanel.Controls.Count; i++)
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
            if(_enemy is null)
            {
                MessageBox.Show("Enemy doesnt Chosen!");
                return;
            }

            Console.Write(_enemy);
            MessageBox.Show("Game can be started!", "Success");

        }
    }
}
