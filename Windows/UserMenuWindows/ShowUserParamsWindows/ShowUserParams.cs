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

namespace ChessDiploma.Windows.UserMenuWindows.ShowUserParamsWindows
{
    public partial class ShowUserParams : Form
    {
        private User _user;
        public ShowUserParams(User chosenUser)
        {
            _user = chosenUser;

            InitializeComponent();
            FillParams();

        }
        public void FillParams()
        {
            LoginLB.Text = _user.Login;
            EmailLB.Text = _user.Email;
            RaitingLB.Text = _user.Rating.ToString();

            WonsLB.Text = _user.Wons.ToString();
            LostsLB.Text = _user.Losts.ToString();
            DrawsLB.Text = _user.Draws.ToString();
            GamesAmountLB.Text = (_user.Wons + _user.Losts + _user.Draws).ToString();
        }
        private void BackBut_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
