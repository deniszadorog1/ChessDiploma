using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using ChessLib;
using ChessDiploma.Models;
using ChessLib.Figures;
using ChessLib.Enums.Players;
using ChessLib.Other;

namespace ChessDiploma.Windows
{
    public partial class FieldFrom : Form
    {
        private (int, int) _fieldCellSize = (50, 50);
        private readonly int _amountOfSquaresInField = Data._game.GetFieldLegthParam();
        private readonly int _amountOfRows = Data._game.GetFieldSizeParam();

        private Color _firstCellColor = Color.FromArgb(121, 72, 57);//first color
        private Color _secondCellColor = Color.FromArgb(93, 50, 49);//second color
        private Color _borderColor = Color.FromArgb(56, 56, 56);//BorderColor

        private Font _defaultFont = new Font("Times New Roman", 22);

        private bool _fillingFieldMarker = true;

        private Panel _fieldPanel = new Panel();
        private (int, int) _fieldBorederSize = (20, 20);
        private readonly (int, int) _firstCellLock = (10, 10);

        private Panel _mainMenuPanel = new Panel();
        private Size _mainMenuSize;
        private Point _mainMenuLocation = new Point(0, 0);

        private Panel _inGameMenu = new Panel();
        private Size _inGameMenuSize;
        private Point _inGameMenuLocation;

        private Panel _firstPlayerPanel = new Panel();
        private Size _firstPlayerPanelSize;
        private Point _firstPlayerPlanelLocation;

        private Panel _secondPlayerPanel = new Panel();
        private Size _secondPlayerPanelSize;
        private Point _secondPlanelLocation;

        private const int _indentPlayerPanelBorder = 10;

        public PictureBox[,] _field;
        private List<Image> _images = new List<Image>();
        private Color[,] _originalColors;

        private AllMoves _moves = new AllMoves();

        public FieldFrom()
        {
            InitializeComponent();

            InitSizes();

            InitGamePanels();

            InitFieldPanel();
            //InitCellsInField();

            GetImagesPath();

            InitFieldArrSize();
        }

        public void InitFieldArrSize()
        {
            _field = new PictureBox[_amountOfRows, _amountOfRows];
            _originalColors = new Color[_amountOfRows, _amountOfRows];
            int cellSizePar = _fieldCellSize.Item1;
            int heightIters = 0;
            (int, int) tempLock = _firstCellLock;
            for (int i = 0; i < _field.GetLength(0); i++)
            {
                for(int j = 0; j < _field.GetLength(1); j++)
                {
                    _field[i, j] = new PictureBox();
                    Figure figure = Data._game.InitCord((i, j));
                    
                    if(j == 0)
                    {
                        tempLock = _firstCellLock;
                        int heightMultyplier = cellSizePar * heightIters;
                        tempLock = (_firstCellLock.Item1, tempLock.Item2 + heightMultyplier);
                        AddPictureBox(GetTempColor(), tempLock, (i,j));
                        heightIters++;
                    }
                    else
                    {
                        tempLock = (tempLock.Item1 + cellSizePar, tempLock.Item2);
                        AddPictureBox(GetCellColor(), tempLock, (i,j));
                    }
                    InitImageInCell(GetImageForCell(figure), (i, j));
                }
            }
        }
        private void InitImageInCell(Image img, (int x,int y) cord)
        {
            _field[cord.x, cord.y].Image = img;
            _field[cord.x, cord.y].SizeMode = PictureBoxSizeMode.StretchImage;
        }


        private Image GetImageForCell(Figure fig)
        {
            return fig is null ? null :
                fig is Pawn && fig.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhitePawn") :
                fig is Pawn && fig.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackPawn") :

                fig is Rook && fig.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteRook") :
                fig is Rook && fig.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackRook") :

                fig is Horse && fig.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteHorse") :
                fig is Horse && fig.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackHorse") :

                fig is Bishop && fig.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteBishop") :
                fig is Bishop && fig.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackBishop") :

                fig is King && fig.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteKing") :
                fig is King && fig.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackKing") :

                fig is Queen && fig.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteQueen") :
                fig is Queen && fig.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackQueen") : null;
        }
        
        private void GetImagesPath()
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string imageDirectory = baseDirectoryInfo.Parent.Parent.FullName;
            string imagePath = Path.Combine(imageDirectory, "Images");

            string whiteFigurePath = Path.Combine(imagePath, "WhiteFigures");
            AddImage(Path.Combine(whiteFigurePath, "WhitePawn.png"), "WhitePawn");
            AddImage(Path.Combine(whiteFigurePath, "WhiteRook.png"), "WhiteRook");
            AddImage(Path.Combine(whiteFigurePath, "WhiteHorse.png"), "WhiteHorse");
            AddImage(Path.Combine(whiteFigurePath, "WhiteBishop.png"), "WhiteBishop");
            AddImage(Path.Combine(whiteFigurePath, "WhiteQueen.png"), "WhiteQueen");
            AddImage(Path.Combine(whiteFigurePath, "WhiteKing.png"), "WhiteKing");

            string blackFigurePath = Path.Combine(imagePath, "BlackFigures");
            AddImage(Path.Combine(blackFigurePath, "BlackPawn.png"), "BlackPawn");
            AddImage(Path.Combine(blackFigurePath, "BlackRook.png"), "BlackRook");
            AddImage(Path.Combine(blackFigurePath, "BlackHorse.png"), "BlackHorse");
            AddImage(Path.Combine(blackFigurePath, "BlackBishop.png"), "BlackBishop");
            AddImage(Path.Combine(blackFigurePath, "BlackQueen.png"), "BlackQueen");
            AddImage(Path.Combine(blackFigurePath, "BlackKing.png"), "BlackKing");
        }

        private void AddImage(string path, string name)
        {
            Image newImage = Image.FromFile(path);
            newImage.Tag = name;

            _images.Add(newImage);
        }
        public void InitSizes()
        {
            //Main menu params
            _mainMenuSize.Width = Width / 4;
            _mainMenuSize.Height = Height;

            //InGame menu params
            _inGameMenuSize.Width = Width / 4;
            _inGameMenuSize.Height = Height;
            _inGameMenuLocation.X = Width / 4 * 3;
            _inGameMenuLocation.Y = 0;

            //field params
            int sizeParam = _fieldCellSize.Item1 * (_amountOfSquaresInField /
                _amountOfRows) + _fieldBorederSize.Item1; //420
            _fieldPanel.Size = new Size(sizeParam, sizeParam);
            _fieldPanel.Location = new Point(Width / 2 - sizeParam / 2, Height / 2 - sizeParam / 2);

            //first player params
            int playerPanelWidthParam = (int)(_fieldPanel.Width * 1.25);
            int playerPanelHeightParam = _fieldPanel.Location.Y - _indentPlayerPanelBorder * 2;

            int firstPlayerPanelYLocation = _fieldPanel.Location.Y + _fieldPanel.Size.Height + _indentPlayerPanelBorder;
            _firstPlayerPanelSize = new Size(playerPanelWidthParam, playerPanelHeightParam);
            _firstPlayerPlanelLocation = new Point(Width / 2 - _firstPlayerPanelSize.Width / 2, firstPlayerPanelYLocation);

            //second player params
            _secondPlayerPanelSize = new Size(playerPanelWidthParam, playerPanelHeightParam);
            _secondPlanelLocation = new Point(Width / 2 - _firstPlayerPanelSize.Width / 2, _indentPlayerPanelBorder);
        }

        //CREATING FIELD 
        //FIELD - Panel
        public void InitFieldPanel()
        {
            //Size - 8 * 8 cells + borders for numbers and letters(10px)
            _fieldPanel.BorderStyle = BorderStyle.FixedSingle;
            _fieldPanel.BackColor = _borderColor;
            _fieldPanel.AutoScroll = false;

            Controls.Add(_fieldPanel);
        }

        public Color GetTempColor()
        {
            return _fillingFieldMarker ? _firstCellColor : _secondCellColor;
        }
        public Color GetCellColor()
        {
            if (!_fillingFieldMarker)
            {
                _fillingFieldMarker = !_fillingFieldMarker;
                return _firstCellColor;
            }
            _fillingFieldMarker = !_fillingFieldMarker;
            return _secondCellColor;
        }
        public void AddPictureBox(Color color, (int, int) loc, (int x,int y) cord)
        {
            _field[cord.x, cord.y].BorderStyle = BorderStyle.FixedSingle;
            _field[cord.x, cord.y].BackColor = color;
            _field[cord.x, cord.y].Size = new Size(50, 50);
            _field[cord.x, cord.y].Location = new Point(loc.Item1, loc.Item2);
            _field[cord.x, cord.y].BorderStyle = BorderStyle.None;

            _originalColors[cord.x, cord.y] = color;

            _field[cord.x, cord.y].Click += (sender, e) =>
            {
                int moveIndex = GetMoveIndexToMake(cord);

                if (_field[cord.x, cord.y].BackColor == Color.Yellow && moveIndex != -1)
                {
                    //reassign in logic
                    Data._game.ReassignMove(_moves.PossibleMoves[moveIndex]);
                   
                    //reassign in form
                    ReassignMove(_moves.PossibleMoves[moveIndex]);

                    AssignOriginalColors();
                    return;
                }
                AssignOriginalColors();
                if (!(_field[cord.x, cord.y].Image is null))
                {
                    _field[cord.x, cord.y].BackColor = Color.Yellow;
                    _moves = Data._game.GetMovesForFigure(cord);
                    ShowEndPointsToMove();
                    Console.WriteLine();
                }
            };
            _fieldPanel.Controls.Add(_field[cord.x, cord.y]);
        }
        public void ReassignMove(Move move)
        {
            //Move reassigned in logic 
            Image toFill = null;
            for(int i = 0; i < move.OneMove.Count; i++)
            {
                (int x, int y) cord = move.OneMove[i];
                if (i % 2 == 0 || i == 0)
                {
                    toFill = _field[cord.x, cord.y].Image;

                    _field[cord.x, cord.y].Image = null;
                }
                else
                {
                    _field[cord.x, cord.y].Image = toFill;
                    //fill cell
                }
            }
        }
        public int GetMoveIndexToMake((int x,int y) cord)
        {
            for(int i = 0; i < _moves.PossibleMoves.Count; i++)
            {
                for(int j = 1; j < _moves.PossibleMoves[i].OneMove.Count; j++)
                {
                    if (_moves.PossibleMoves[i].OneMove[j] == cord)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private void ShowEndPointsToMove()
        {
            for(int i = 0; i < _moves.PossibleMoves.Count; i++)
            {
                if (_moves.PossibleMoves[i].OneMove.Count == 2)
                {
                    (int x, int y) endMovePoint = _moves.PossibleMoves[i].OneMove.Last();
                    _field[endMovePoint.x, endMovePoint.y].BackColor = Color.Yellow;
                }
            }
        }
        private void AssignOriginalColors()
        {
            for(int i = 0; i < _field.GetLength(0); i++)
            {
                for(int j = 0; j < _field.GetLength(1); j++)
                {
                    if (_field[i,j].BackColor != _originalColors[i, j])
                    {
                        _field[i, j].BackColor = _originalColors[i,j];
                    }
                }
            }
        }
        public void InitGamePanels()
        {
            InitMainMenuPanel();
            InitGameMenuPanel();

            InitFirstPlayerPanel();
            InitSecondPlayerPanel();
        }
        /// <summary>
        /// Main menu panel initialization
        /// </summary>
        public void InitMainMenuPanel()
        {
            _mainMenuPanel.BorderStyle = BorderStyle.FixedSingle;
            _mainMenuPanel.Location = _mainMenuLocation;
            _mainMenuPanel.Size = _mainMenuSize;
            _mainMenuPanel.Name = "MainMenu";
            AddDefaultLabelToPanel(_mainMenuPanel, "Main Menu");

            Controls.Add(_mainMenuPanel);
        }
        /// <summary>
        /// InGame menu initialization
        /// </summary>
        public void InitGameMenuPanel()
        {
            _inGameMenu.BorderStyle = BorderStyle.FixedSingle;
            _inGameMenu.Location = _inGameMenuLocation;
            _inGameMenu.Size = _inGameMenuSize;
            _inGameMenu.Name = "InGameMenu";
            AddDefaultLabelToPanel(_inGameMenu, "InGame Menu");

            Controls.Add(_inGameMenu);
        }
        /// <summary>
        /// Init first player panel
        /// </summary>
        public void InitSecondPlayerPanel()
        {
            _firstPlayerPanel.BorderStyle = BorderStyle.FixedSingle;
            _firstPlayerPanel.Size = _firstPlayerPanelSize;
            _firstPlayerPanel.Location = _firstPlayerPlanelLocation;
            _firstPlayerPanel.Name = "FirstPlayerPanel";
            AddDefaultLabelToPanel(_firstPlayerPanel, "First player panel");

            Controls.Add(_firstPlayerPanel);
        }
        /// <summary>
        /// Init second player panel
        /// </summary>
        public void InitFirstPlayerPanel()
        {
            _secondPlayerPanel.BorderStyle = BorderStyle.FixedSingle;
            _secondPlayerPanel.Size = _secondPlayerPanelSize;
            _secondPlayerPanel.Location = _secondPlanelLocation;
            _secondPlayerPanel.Name = "SecondPlayerPanel";
            AddDefaultLabelToPanel(_secondPlayerPanel, "Second player panel");

            Controls.Add(_secondPlayerPanel);
        }
        public void AddDefaultLabelToPanel(Panel panel, string toPrint)
        {
            Label defaultLabel = new Label();
            defaultLabel.Font = _defaultFont;
            defaultLabel.Text = toPrint;
            defaultLabel.AutoSize = true;

            panel.Controls.Add(defaultLabel);

            defaultLabel.Location = new Point((panel.Width - defaultLabel.Width) / 2,
                (panel.Height - defaultLabel.Height) / 2);

        }
        private void FieldFrom_Load(object sender, EventArgs e)
        {

        }
    }
}
