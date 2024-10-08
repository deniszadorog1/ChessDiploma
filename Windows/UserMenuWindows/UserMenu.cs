﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ChessLib.PlayerModels;
using ChessDiploma.Windows.UserMenuWindows.ShowGameWindows;
using ChessDiploma.Models;


namespace ChessDiploma.Windows.UserMenuWindows
{
    public partial class UserMenu : Form
    {
        private User _user;
        private List<User> _allUsers;
        public UserMenu(User user, List<User> allUsers)
        {
            _user = user;
            _allUsers = allUsers;
            InitializeComponent();

            FillMainParamsPanel();
            FillOtherParamsPanel();
        }
        public void FillMainParamsPanel()
        {
            LoginLB.Text = _user.Login;
            EmailLb.Text = _user.Email;
            RatingLB.Text = _user.Rating.ToString();
        }
        public void FillOtherParamsPanel()
        {
            GamesAmountLB.Text = (_user.Wons + _user.Draws + _user.Losts).ToString();
            WonsLB.Text = _user.Wons.ToString();
            LostsLB.Text = _user.Losts.ToString();
            DrawsLB.Text = _user.Draws.ToString();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PlayGame_Click(object sender, EventArgs e)
        {
            Hide();
            PlayGameParams playGame = new PlayGameParams(_user, _allUsers);
            playGame.ShowDialog();
            Show();

            FillMainParamsPanel();
            FillOtherParamsPanel();
        }
        private void SeeAllPlayers_Click(object sender, EventArgs e)
        {
            Hide();
            ChoosePlayer choose = new ChoosePlayer(_allUsers);
            choose.ShowDialog();
            Show();
        }

        private void SeeAllGames_Click(object sender, EventArgs e)
        {
            Hide();
            ShowGames gameReplay = new ShowGames();
            gameReplay.ShowDialog();
            Show();
        }
        private void CorrectAccount_Click(object sender, EventArgs e)
        {
            User copy = new User(_user.Email, _user.Login, _user.Password, _user.DateBirth);

            UserCorrection correct = new UserCorrection(_user, _allUsers);
            correct.ShowDialog();

            DbUsage.UpdateUser(copy.Email, copy.Login, copy.Password, _user);

            LoginLB.Text = _user.Login;
            EmailLb.Text = _user.Email;
        }
        private void SendStat_Click(object sender, EventArgs e)
        {
            EmailSender emailSender = new EmailSender(_user);
            bool check = emailSender.SendMessage();

            if (!check)
            {
                MessageBox.Show("cant send email");
            }
            else MessageBox.Show("sent!");

        }
    }
}
