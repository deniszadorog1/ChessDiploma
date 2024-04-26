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

namespace ChessDiploma.Windows
{
    public partial class CreateAccount : Form
    {
        public User _newUser = null;
        private List<User> _users = new List<User>();
        public CreateAccount(List<User> users)
        {
            InitializeComponent();
            for (int i = 0; i < users.Count; i++)
            {
                _users.Add(users[i]);
            }
        }
        private void CreateBut_Click(object sender, EventArgs e)
        {
            if(EmailBox.Text == "" || LoginBox.Text == "" || PasswordBox.Text == "" || 
               _users.Exists(x => x.Login == LoginBox.Text))
            {
                MessageBox.Show("Cant be add!", "Mistake!");
                return;
            }

            DateTime birth = DateBirth.Value;
            _newUser = new User(EmailBox.Text, LoginBox.Text, PasswordBox.Text, birth);
            DbUsage.InsertPlayer(_newUser);
            _users.Add(_newUser);
            MessageBox.Show("Account created!", "Success!");
            Close();
        }
        private void BackBut_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
