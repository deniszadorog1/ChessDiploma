using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ChessDiploma.Windows;
using ChessLib.PlayerModels;
using ChessDiploma.Models;
using ChessDiploma.Windows.UserMenuWindows;

namespace ChessDiploma
{
    public partial class StartForm : Form
    {
        private Panel _leftPanel = new Panel();
        Color _leftPanelColor = Color.FromArgb(173, 216, 230);
        Color _leftPanelTextColor = Color.White;

        private Panel _rightPanel = new Panel();
        private const int _enterPanelSpaces = 15;

        private Font _loginPanelLBsFont = new Font("Times New Roman", 14);
        private const int _spaceInEnterLoginPanel = 5;

        private Button _loginIn;
        private Button _createAccount;

        private TextBox _enterPawssword;
        private TextBox _enterLogin;

        private List<User> _users = DbUsage.GetAllUsers();

        private const int _butWidth = 145;
        private const int _butHeight = 50;
        private const int _enterPanelHeight = 150;
        private const int _halfDevider = 2;



        public StartForm()
        {
            InitializeComponent();

            InitPanels();

            FillLeftStartPanel();
            FillRightPanel();
        }
        public void InitPanels()
        {
            _leftPanel.Location = new Point(0, 0);
            _leftPanel.Size = new Size(Width / 2, Height);
            _leftPanel.BackColor = _leftPanelColor;
            //_leftPanel.BorderStyle = BorderStyle.FixedSingle;

            _rightPanel.Location = new Point(Width / 2, 0);
            _rightPanel.Size = new Size(Width / 2, Height);
            //_rightPanel.BorderStyle = BorderStyle.FixedSingle;

            Controls.Add(_leftPanel);
            Controls.Add(_rightPanel);
        }

        public void FillLeftStartPanel()
        {
            Label shortProgName = new Label();
            shortProgName.ForeColor = _leftPanelTextColor;
            shortProgName.Text = "Chess program";
            shortProgName.Font = new Font("Times New Roman", 32);
            shortProgName.Location = new Point(0, 
                _leftPanel.Height / _halfDevider - shortProgName.Height);
            shortProgName.AutoSize = true;

            Label programName = new Label();
            programName.ForeColor = _leftPanelTextColor;
            programName.Font = new Font("Times New Roman", 24);
            programName.Location = new Point(0, shortProgName.Location.Y + shortProgName.Size.Height);
            programName.AutoSize = true;
            programName.Location = new Point(programName.Location.X, 
                programName.Location.Y + programName.Height);
            programName.Text = "Application for training the game of chess";

            _leftPanel.Controls.Add(shortProgName);
            _leftPanel.Controls.Add(programName);

            Controls.Add(_leftPanel);
        }


        public void FillRightPanel()
        {
            Panel enterPanel = new Panel();

            enterPanel.Size = new Size(Width / _halfDevider - _enterPanelSpaces * 
                _halfDevider, _enterPanelHeight);
            //enterPanel.BorderStyle = BorderStyle.FixedSingle;
            enterPanel.Location = new Point(_enterPanelSpaces, 
                Height / _halfDevider - enterPanel.Height / _halfDevider);

            Label loginLB = new Label();
            InitParamsForEnterLogingPanelLBs(loginLB, "Login");
            loginLB.Location = new Point(0, 0);
            _enterLogin = new TextBox();
            _enterLogin.Location = new Point(0, loginLB.Bottom);
            InitParamsForEnterLoginTextBoxes(_enterLogin);


            Label enterPasswordLB = new Label();
            InitParamsForEnterLogingPanelLBs(enterPasswordLB, "Password");
            enterPasswordLB.Location = new Point(0, _enterLogin.Bottom);
            _enterPawssword = new TextBox();
            _enterPawssword.Location = new Point(0, enterPasswordLB.Bottom);
            _enterPawssword.PasswordChar = '*';
            InitParamsForEnterLoginTextBoxes(_enterPawssword);

            _loginIn = new Button();
            InitParamsForButtonsInLoginPanel(_loginIn, "Log in");
            _loginIn.Location = new Point(0, _enterPawssword.Bottom);
            _loginIn.Click += LoingIn_Click;

            _createAccount = new Button();
            InitParamsForButtonsInLoginPanel(_createAccount, "Create account");
            _createAccount.Location = new Point(_loginIn.Width + _spaceInEnterLoginPanel * 
                _halfDevider, _loginIn.Location.Y);
            _createAccount.Click += CreateAccount_Click;

            enterPanel.Controls.Add(loginLB);
            enterPanel.Controls.Add(_enterLogin);
            enterPanel.Controls.Add(enterPasswordLB);
            enterPanel.Controls.Add(_enterPawssword);
            enterPanel.Controls.Add(_loginIn);
            enterPanel.Controls.Add(_createAccount);

            _rightPanel.Controls.Add(enterPanel);
        }
        private void LoingIn_Click(object sender, EventArgs e)
        {
            string login = _enterLogin.Text;
            string password = _enterPawssword.Text;

            User user = _users.Find(x => x.Login == login && x.Password == password);

            if (!(user is null))
            {
                _enterPawssword.Text = "";
                Hide();
                UserMenu userMenu = new UserMenu(user, _users);
                userMenu.ShowDialog();
                Show();
            }
            else MessageBox.Show("No user with such login!");

        }
        private void CreateAccount_Click(object sender, EventArgs e)
        {
            CreateAccount create = new CreateAccount(_users);
            create.ShowDialog();
            _users = DbUsage.GetAllUsers();
        }

        public void InitParamsForButtonsInLoginPanel(Button but, string text)
        {
            but.Size = new Size(_butWidth, _butHeight - _spaceInEnterLoginPanel);
            but.Text = text;
            but.Font = _loginPanelLBsFont;
        }
        public void InitParamsForEnterLogingPanelLBs(Label label, string text)
        {
            label.Text = text;
            label.AutoSize = true;
            label.Font = _loginPanelLBsFont;
        }
        public void InitParamsForEnterLoginTextBoxes(TextBox box)
        {
            const int maxLengthInTextBox = 32;
            const int boxWidth = 300;

            box.Font = new Font("Times new Roman", 14);
            box.MaxLength = maxLengthInTextBox;
            box.Width = boxWidth;
            box.BackColor = SystemColors.Control;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            FieldFrom game = new FieldFrom();
            game.ShowDialog();

        }
    }
}
