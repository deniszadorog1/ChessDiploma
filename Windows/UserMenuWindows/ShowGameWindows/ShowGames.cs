using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ChessLib;
using ChessDiploma.Models;
using ChessLib.Enums.Game;

namespace ChessDiploma.Windows.UserMenuWindows.ShowGameWindows
{
    public partial class ShowGames : Form
    {
        private List<Game> _allGames = DbUsage.GetAllGames();
        private Game _chosenGame;
        private const int _distanceBetweenLabels = 25;
        private const int _gameLabelHeight = 200;

        public ShowGames()
        {
            InitializeComponent();
            FillGamesPanel();
        }
        public void FillGamesPanel()
        {
            GamesPanel.Controls.Clear();
            Point loc = new Point(0, 0);
            for (int i = 0; i < _allGames.Count; i++)
            {
                //first player login 
                //second player login 
                //start date
                //end date

                Label label = new Label();
                label.AutoSize = false;
                label.Location = loc;
                label.Size = new Size(GamesPanel.Width, _gameLabelHeight);

                string players = _allGames[i].Players[0].Login + " VS " + _allGames[i].Players[1].Login;
                string startDate = _allGames[i].StartTime.Date.ToString();
                string endDate = _allGames[i].EndTime.ToString();

                label.Text = players + "\n" + startDate + "\n" + endDate;
                label.Font = new Font("Times New Romsn", 14);

                label.Click += GameLabel_Click;

                GamesPanel.Controls.Add(label);

                loc = new Point(0, loc.Y + _distanceBetweenLabels + label.Size.Height);
            }
        }
        private void GameLabel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < GamesPanel.Controls.Count; i++)
            {
                if (GamesPanel.Controls[i] is Label)
                {
                    GamesPanel.Controls[i].ForeColor = Color.Black;
                }
            }
            if (sender is Label label)
            {
                label.ForeColor = Color.Green;
                ChosenGameLB.Text = label.Text;
                InitChosenGame();
            }
        }
        private void InitChosenGame()
        {
            for(int i = 0; i < GamesPanel.Controls.Count; i++)
            {
                if (GamesPanel.Controls[i].ForeColor != Color.Black)
                {
                    _chosenGame = _allGames[i];
                }
            }
        }
        private void WatchBut_Click(object sender, EventArgs e)
        {
            FieldFrom gameReplay = new FieldFrom(_chosenGame, ReplayOrGame.Replay);
            gameReplay.ShowDialog();
        }
        private void BackBut_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
