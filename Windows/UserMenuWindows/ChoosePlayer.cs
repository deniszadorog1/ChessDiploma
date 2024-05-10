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
using ChessDiploma.Windows.UserMenuWindows.ShowUserParamsWindows;

namespace ChessDiploma.Windows.UserMenuWindows
{
    public partial class ChoosePlayer : Form
    {
        private List<User> _users = new List<User>();
        private User _chosenUser = new User();

        private const int _distansBetweenLogins = 25;
        private const int _butWidthError = 15;
        private const int _butHeight = 50;
        public ChoosePlayer(List<User> users)
        {
            _users = users;
            InitializeComponent();

            FillPlayersList("");
        }
        public void FillPlayersList(string serchText)
        {
            PlayersList.Controls.Clear();
            Point loc = new Point(0, 0);
            for (int i = 0; i < _users.Count; i++)
            {
                if (serchText == "")
                {
                    loc = CreateUserButtons(_users[i], loc);
                }
                else if (_users[i].Login.Contains(serchText))
                {
                    loc = CreateUserButtons(_users[i], loc);
                }

            }
        }
        public Point CreateUserButtons(User printUser, Point location)
        {
            Button user = new Button();
            user.Text = printUser.Login;
            user.Font = new Font("Times New Roman", 14);
            user.Click += Player_Click;
            user.Location = location;
            user.Size = new Size(PlayersList.Width - _butWidthError, _butHeight);
            PlayersList.Controls.Add(user);
            return new Point(0, location.Y + _distansBetweenLogins);
        }

        private void Player_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PlayersList.Controls.Count; i++)
            {
                PlayersList.Controls[i].ForeColor = Color.Black;
            }
            if (sender is Button clickedLabel)
            {
                clickedLabel.ForeColor = Color.Green;
                _chosenUser = _users.Find(x => x.Login == clickedLabel.Text);
                ChosenPlayerLB.Text = _chosenUser.Login;
            }
        }
        private void BackBut_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShowParamsBut_Click(object sender, EventArgs e)
        {
            if (_chosenUser is null)
            {
                MessageBox.Show("User doesnt chose!", "Mistake!");
                return;
            }
            ShowUserParams showParams = new ShowUserParams(_chosenUser);
            showParams.ShowDialog();
        }
        private void Searcher_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox box)
            {
                FillPlayersList(box.Text);
            }
        }
    }
}
