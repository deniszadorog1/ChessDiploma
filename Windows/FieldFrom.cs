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


namespace ChessDiploma.Windows
{
    public partial class FieldFrom : Form
    {
        private (int, int) _startPotintToEnter = (250, 250);
        private (int, int) _fieldPartSize = (50, 50);
        private const int _amountOfSquaresInField = 64; 

        public FieldFrom()
        {
            InitializeComponent();
        }

        public void EnterField()
        {
            (int, int) tempPointToPrint = _startPotintToEnter;
            int heightIters = 0;
            for (int i = 0; i < _amountOfSquaresInField; i++)
            {
                if(i % 8 == 0)
                {

                }
                else
                {

                }
            }
        }
        public void AddPictureBox(Color color, (int,int) loc)
        {
            PictureBox newCell = new PictureBox();
            newCell.BorderStyle = BorderStyle.FixedSingle;
            newCell.BackColor = color;
            newCell.Size = new Size(50, 50);
            newCell.Location = new Point(loc.Item1, loc.Item2);
        }

    }
}
