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
    public partial class UserCorrection : Form
    {
        private User _user;
        private List<User> _allUsers;
        public UserCorrection(User user, List<User> users)
        {
            _user = user;
            _allUsers = users;
            InitializeComponent();

            FillBoxes();
        }
        public void FillBoxes()
        {
            EmailBox.Text = _user.Email;
            LoginBox.Text = _user.Login;
            PasswordBox.Text = _user.Password;
        }

        private void BackBut_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CorrectBut_Click(object sender, EventArgs e)
        {
            if(_allUsers.Exists(x => x.Email == EmailBox.Text || x.Login == LoginBox.Text) || 
                LoginBox.Text == "" || EmailBox.Text == "" || PasswordBox.Text == "")
            {
                MessageBox.Show("Cant be changed!", "Mistake!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _user.Email = EmailBox.Text;
            _user.Login = LoginBox.Text;
            _user.Password = PasswordBox.Text;
            _user.DateBirth = DateBirth.Value;
            MessageBox.Show("Changed!", "Success!");
            Close();
        }
    }
}
