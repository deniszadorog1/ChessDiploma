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
using ChessDiploma.Models;

namespace ChessDiploma.Windows.StartWindows
{
    public partial class FAAuth : Form
    {
        User _user;
        public bool res;
        public FAAuth(User user)
        {
            _user = user;
            InitializeComponent();
            SendCodeForTwoFA.SendMessage(_user.PhoneNumber);
        }
        private void FAAuth_Load(object sender, EventArgs e)
        {

        }
        private void EnterBut_Click(object sender, EventArgs e)
        {
            if(SendCodeForTwoFA._code == CodeBox.Text)
            {
                res = true;
                Close();
                return;
            }
            else
            {
                MessageBox.Show("Wrong code!");
            }
        }

        private void CloseBut_Click(object sender, EventArgs e)
        {
            res = false;
            Close();
        }
    }
}
