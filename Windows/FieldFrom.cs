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

using ChessLib.Enums.Figures;
using ChessDiploma.Models;
using ChessLib.Figures;
using ChessLib.Enums.Players;
using ChessLib.Other;
using ChessLib.PlayerModels;

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

        private Panel _convertWhite = new Panel();
        private List<Image> _whiteFigsImagesToConvert = new List<Image>();

        private Panel _convertBlack = new Panel();
        private List<Image> _blackFigsImagesToConvert = new List<Image>();

        private const int _indentPlayerPanelBorder = 10;

        public PictureBox[,] _field;
        private List<Image> _images = new List<Image>();

        private string _whiteFiguresPath = "";
        private string _blackFiguresPath = "";

        private Color[,] _originalColors;

        private AllMoves _moves = new AllMoves();
        private string _convertFigName = "";

        public FieldFrom()
        {
            InitializeComponent();

            InitSizes();
            InitGamePanels();
            InitFieldPanel();
            GetImagesPath();
            InitFieldArrSize();

            FillWhiteFiguresImagesToConvertPawn();
            FillBlackFiguresImagesToConvertPawn();

            CreateConvertPawnPanels();
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
                for (int j = 0; j < _field.GetLength(1); j++)
                {
                    _field[i, j] = new PictureBox();
                    Figure figure = Data._game.InitCord((i, j));

                    if (j == 0)
                    {
                        tempLock = _firstCellLock;
                        int heightMultyplier = cellSizePar * heightIters;
                        tempLock = (_firstCellLock.Item1, tempLock.Item2 + heightMultyplier);
                        AddPictureBox(GetTempColor(), tempLock, (i, j));
                        heightIters++;
                    }
                    else
                    {
                        tempLock = (tempLock.Item1 + cellSizePar, tempLock.Item2);
                        AddPictureBox(GetCellColor(), tempLock, (i, j));
                    }
                    InitImageInCell(GetImageForCell(figure), (i, j));
                }
            }
        }
        private void InitImageInCell(Image img, (int x, int y) cord)
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
        private void FillWhiteFiguresImagesToConvertPawn()
        {
            _whiteFigsImagesToConvert.Add(_images.Find(x => x.Tag.ToString() == "WhiteRook"));
            _whiteFigsImagesToConvert.Add(_images.Find(x => x.Tag.ToString() == "WhiteQueen"));
            _whiteFigsImagesToConvert.Add(_images.Find(x => x.Tag.ToString() == "WhiteHorse"));
            _whiteFigsImagesToConvert.Add(_images.Find(x => x.Tag.ToString() == "WhiteBishop"));
        }
        private void FillBlackFiguresImagesToConvertPawn()
        {
            _blackFigsImagesToConvert.Add(_images.Find(x => x.Tag.ToString() == "BlackRook"));
            _blackFigsImagesToConvert.Add(_images.Find(x => x.Tag.ToString() == "BlackQueen"));
            _blackFigsImagesToConvert.Add(_images.Find(x => x.Tag.ToString() == "BlackHorse"));
            _blackFigsImagesToConvert.Add(_images.Find(x => x.Tag.ToString() == "BlackBishop"));
        }
        private void GetImagesPath()
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string imageDirectory = baseDirectoryInfo.Parent.Parent.FullName;
            string imagePath = Path.Combine(imageDirectory, "Images");

            _whiteFiguresPath = Path.Combine(imagePath, "WhiteFigures");
            AddImage(Path.Combine(_whiteFiguresPath, "WhitePawn.png"), "WhitePawn");
            AddImage(Path.Combine(_whiteFiguresPath, "WhiteRook.png"), "WhiteRook");
            AddImage(Path.Combine(_whiteFiguresPath, "WhiteHorse.png"), "WhiteHorse");
            AddImage(Path.Combine(_whiteFiguresPath, "WhiteBishop.png"), "WhiteBishop");
            AddImage(Path.Combine(_whiteFiguresPath, "WhiteQueen.png"), "WhiteQueen");
            AddImage(Path.Combine(_whiteFiguresPath, "WhiteKing.png"), "WhiteKing");

            _blackFiguresPath = Path.Combine(imagePath, "BlackFigures");
            AddImage(Path.Combine(_blackFiguresPath, "BlackPawn.png"), "BlackPawn");
            AddImage(Path.Combine(_blackFiguresPath, "BlackRook.png"), "BlackRook");
            AddImage(Path.Combine(_blackFiguresPath, "BlackHorse.png"), "BlackHorse");
            AddImage(Path.Combine(_blackFiguresPath, "BlackBishop.png"), "BlackBishop");
            AddImage(Path.Combine(_blackFiguresPath, "BlackQueen.png"), "BlackQueen");
            AddImage(Path.Combine(_blackFiguresPath, "BlackKing.png"), "BlackKing");
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
        public void AddPictureBox(Color color, (int, int) loc, (int x, int y) cord)
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
                    Data._game.IfSpecialFigIsMoves(_moves.PossibleMoves[moveIndex]);
                    if (!(_moves.PossibleMoves[moveIndex].HitFigure is null))
                    {
                        Figure fig = _moves.PossibleMoves[moveIndex].HitFigure;
                        Data._game.AddHitFigure(fig);
                        InitHitFiguresPanel(GetHitFigsPanel());
                    }

                    AssignOriginalColors();

                    if (Data._game.IfSteperCheckMated())
                    {
                        MessageBox.Show("Game ended. " + Data._game._steper.Name + " WON", "Game ended!", MessageBoxButtons.OK);
                        Close();
                    }
                    else if (Data._game.IfGameEndedByPate())
                    {
                        MessageBox.Show("Game ended. Its Draw(Pate)");
                        Close();
                    }
                    if (Data._game.IfPawnCameToTheEndOfBoard())
                    {
                        ShowConvertPanel(cord);
                    }
                    else
                    {
                        Data._game.ChangeSteper();
                    }
                    return;
                }
                AssignOriginalColors();
                if (!(_field[cord.x, cord.y].Image is null) && Data._game.IfFigureIsStepers(cord))
                {
                    _field[cord.x, cord.y].BackColor = Color.Yellow;
                    _moves = new AllMoves();
                    _moves = Data._game.GetMovesForFigure(cord);
                    ShowEndPointsToMove();
                    Console.WriteLine();
                }
            };
            _fieldPanel.Controls.Add(_field[cord.x, cord.y]);
        }

        public void ShowConvertPanel((int x, int y) cord)
        {
            PlayerColor color = Data._game.GetColorToConvert();
            PlayerSide stepSide = Data._game.GetStepperSide();
            ShowConvertPanel(color);
            if (color == PlayerColor.White)
            {
                InitLocationForPanel(_convertWhite, stepSide, cord);
            }
            else//black
            {
                InitLocationForPanel(_convertBlack, stepSide, cord);
            }
        }

        public void InitLocationForPanel(Panel panel, PlayerSide side, (int x, int y) cord)
        {
            Point point = _field[cord.x, cord.y].Location;
            int pointX = GetXLocationForConvertPanel(cord, panel.Width, point.X);
            if (side == PlayerSide.Up)
            {
                panel.Location = new Point(pointX, point.Y - _fieldCellSize.Item2);
            }
            else//side - down
            {
                panel.Location = new Point(pointX, point.Y + _fieldCellSize.Item2);
            }
        }
        private List<int> _leftXConvertErrorCords = new List<int>() { 0, 1 };
        private List<int> _rightXConvertErrorCords = new List<int>() { 6, 7 };
        public int GetXLocationForConvertPanel((int y, int x) cord, int panelWidth, int pointX)
        {
            if (!_leftXConvertErrorCords.Contains(cord.x) &&
                !_rightXConvertErrorCords.Contains(cord.x))
            {
                return pointX + _fieldCellSize.Item1 / 2 - panelWidth / 2;
            }
            else if (_leftXConvertErrorCords.Contains(cord.x))
            {
                return pointX;
            }
            else // cordX - 6 || 7
            {
                return pointX - _fieldCellSize.Item1 * 3;
            }

            //return 0;
        }
        public void ShowConvertPanel(PlayerColor color)
        {
            if (color == PlayerColor.White)
            {
                _convertWhite.Show();
                _convertWhite.BringToFront();
            }
            else//black color
            {
                _convertBlack.Show();
                _convertBlack.BringToFront();
            }
        }

        public void CreateConvertPawnPanels()
        {
            //white
            InitParamsForConvertPanel(_convertWhite, _whiteFigsImagesToConvert, "convertWhite");

            //black
            InitParamsForConvertPanel(_convertBlack, _blackFigsImagesToConvert, "convertBlack");

            _fieldPanel.Controls.Add(_convertWhite);

            _fieldPanel.Controls.Add(_convertBlack);
            //BringConvertPanelsToFront();
        }
        public void InitParamsForConvertPanel(Panel convertPanel, List<Image> imagesToFill, string name)
        {
            convertPanel.Name = name;
            int cellParam = _fieldCellSize.Item1;
            int amountOfFigures = Data._game.GetAmountFiguresThatCanBeConverted();
            convertPanel.Size = new Size(amountOfFigures * cellParam, cellParam);

            Point location = new Point(0, 0);
            for (int i = 0; i < amountOfFigures; i++)
            {
                PictureBox picBox = new PictureBox();
                picBox.Size = new Size(_fieldCellSize.Item1, _fieldCellSize.Item2);
                picBox.BorderStyle = BorderStyle.FixedSingle;
                picBox.Location = location;
                location = new Point(picBox.Location.X + cellParam, picBox.Location.Y);
                picBox.Image = imagesToFill[i];
                picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                picBox.Name = imagesToFill[i].Tag.ToString();

                picBox.Click += (sender, e) =>
                {
                    _convertFigName = ((PictureBox)sender).Name.ToString();

                    Image convertedImg = _images.Find(x => x.Tag.ToString() == _convertFigName);

                    (int, int) figCordToConvert = Data._game.GetFiugerCordToConvert();

                    _field[figCordToConvert.Item1, figCordToConvert.Item2].Image = convertedImg;

                    Data._game.ConvertFigure(figCordToConvert, GetFigureTypeToConvert(convertedImg.Tag.ToString()));

                    Data._game.ClearConvertationVariable();
                    HideConvertPanels();
                    Data._game.ChangeSteper();
                };

                convertPanel.Controls.Add(picBox);
            }
        }
        public ConvertPawn GetFigureTypeToConvert(string figName)
        {
            if (figName == "WhiteQueen" || figName == "BlackQueen") return ConvertPawn.Queen;
            if (figName == "WhiteRook" || figName == "BlackRook") return ConvertPawn.Rook;
            if (figName == "WhiteBishop" || figName == "BlackBishop") return ConvertPawn.Bishop;
            if (figName == "WhiteHorse" || figName == "BlackHorse") return ConvertPawn.Horse;
            return new ConvertPawn();
        }
        public void HideConvertPanels()
        {
            _convertBlack.Hide();
            _convertWhite.Hide();
        }
        public void ReassignMove(Move move)
        {
            //Move reassigned in logic 
            Image toFill = null;
            for (int i = 0; i < move.OneMove.Count; i++)
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
                }
            }
        }
        public int GetMoveIndexToMake((int x, int y) cord)
        {
            for (int i = 0; i < _moves.PossibleMoves.Count; i++)
            {
                for (int j = 1; j < _moves.PossibleMoves[i].OneMove.Count; j++)
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
            for (int i = 0; i < _moves.PossibleMoves.Count; i++)
            {
                if (_moves.PossibleMoves[i].OneMove.Count == 2)
                {
                    (int x, int y) endMovePoint = _moves.PossibleMoves[i].OneMove.Last();
                    _field[endMovePoint.x, endMovePoint.y].BackColor = Color.Yellow;
                }
                else
                {
                    (int x, int y) endMovePoint = _moves.PossibleMoves[i].OneMove[1];
                    _field[endMovePoint.x, endMovePoint.y].BackColor = Color.Yellow;
                }
            }
        }
        private void AssignOriginalColors()
        {
            for (int i = 0; i < _field.GetLength(0); i++)
            {
                for (int j = 0; j < _field.GetLength(1); j++)
                {
                    if (_field[i, j].BackColor != _originalColors[i, j])
                    {
                        _field[i, j].BackColor = _originalColors[i, j];
                    }
                }
            }
        }
        public void InitGamePanels()
        {
            InitMainMenuPanel();
            InitGameMenuPanel();

            InitPlayerPanel(_firstPlayerPanel, Data._game.GetFirstPlayerName(), "hitFigsFirst", _firstPlayerPanelSize, _firstPlayerPlanelLocation);
            InitPlayerPanel(_secondPlayerPanel, Data._game.GetLastPlayerName(), "hitFigsSecond", _secondPlayerPanelSize, _secondPlanelLocation);

            Player firstPlayer = Data._game.GetPlayerNameAndColor(0);
            Player secondPlayer = Data._game.GetPlayerNameAndColor(Data._game.Players.Count - 1);

            FillPlayerPlanels(_firstPlayerPanel, firstPlayer);
            FillPlayerPlanels(_secondPlayerPanel, secondPlayer);
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

        public void InitPlayerPanel(Panel panel, string name, string hitFiguresName, Size size, Point location)
        {
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Size = size;
            panel.Location = location;
            panel.Name = name;

            Controls.Add(panel);

            Panel hitFigures = new Panel();
            hitFigures.BorderStyle = BorderStyle.FixedSingle;
            hitFigures.Size = new Size(panel.Width, _fieldCellSize.Item1);
            hitFigures.Location = new Point(0, panel.Size.Height - hitFigures.Height);
            hitFigures.Name = hitFiguresName;

            panel.Controls.Add(hitFigures);

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
        public void FillPlayerPlanels(Panel panel, Player player)
        {
            Label name = new Label();
            name.AutoSize = true;
            name.Font = new Font("Times New Roman", 24);
            name.Text = player.Name + ". Color - " + player.Color.ToString();
            name.Location = new Point(0, 0);
            name.Name = player.Name;

            panel.Controls.Add(name);
        }
        public void InitHitFiguresPanel(Panel hitFigPanel)
        {
            hitFigPanel.Controls.Clear();
            int count = Data._game.GetAmountOfHitFigureList();
            PlayerColor color = Data._game.GetPlayerColor();

            int panelWidth = 75;
            for (int i = 0; i < count; i++)
            {
                (string name, int amount) hitFig = Data._game.GetPlayerHitFigure(i);
                if (hitFig.amount != 0)
                {
                    Panel newPanel = new Panel();
                    newPanel.Size = new Size(panelWidth, _fieldCellSize.Item1);
                    newPanel.Location = new Point(hitFigPanel.Controls.Count * panelWidth, 0);

                    Label amount = new Label();
                    amount.AutoSize = false;
                    amount.Text = hitFig.amount.ToString() + "-";
                    amount.Size = new Size(panelWidth - _fieldCellSize.Item1, _fieldCellSize.Item2);
                    amount.Location = new Point(0, 0);
                    amount.TextAlign = ContentAlignment.MiddleCenter;
                    amount.Font = new Font("Times New Roman", 14);
                    

                    PictureBox figPic = new PictureBox();
                    figPic.SizeMode = PictureBoxSizeMode.StretchImage;
                    figPic.Size = new Size(_fieldCellSize.Item1, _fieldCellSize.Item2);
                    figPic.Location = new Point(panelWidth - _fieldCellSize.Item1, 0);
                    figPic.Image = GetHitFigPictureByName(color, hitFig.name);

                    newPanel.Controls.Add(amount);
                    newPanel.Controls.Add(figPic);


                    hitFigPanel.Controls.Add(newPanel);
                }
            }
        }
        public Image GetHitFigPictureByName(PlayerColor tempPlayerColor, string figName)
        {
            if (tempPlayerColor == PlayerColor.White)
            {
                return figName == "pawn" ? _images.Find(x => x.Tag.ToString() == "BlackPawn") :
                    figName == "rook" ? _images.Find(x => x.Tag.ToString() == "BlackRook") :
                    figName == "horse" ? _images.Find(x => x.Tag.ToString() == "BlackHorse") :
                    figName == "bishop" ? _images.Find(x => x.Tag.ToString() == "BlackBishop") :
                    figName == "queen" ? _images.Find(x => x.Tag.ToString() == "BlackQueen") : null;
            }
            else//Black
            {
                return figName == "pawn" ? _images.Find(x => x.Tag.ToString() == "WhitePawn") :
                    figName == "rook" ? _images.Find(x => x.Tag.ToString() == "WhiteRook") :
                    figName == "horse" ? _images.Find(x => x.Tag.ToString() == "WhiteHorse") :
                    figName == "bishop" ? _images.Find(x => x.Tag.ToString() == "WhiteBishop") :
                    figName == "queen" ? _images.Find(x => x.Tag.ToString() == "WhiteQueen") : null;
            }
        }
        public Panel GetHitFigsPanel()
        {
            Control[] controls = Controls.Find(Data._game._steper.Name, false);

            Panel playerPanel = (Panel)controls.First();

            for (int i = 0; i < playerPanel.Controls.Count; i++)
            {
                if (playerPanel.Controls[i].GetType() == typeof(Panel))
                {
                    return (Panel)playerPanel.Controls[i];
                }
            }
            return null;
        }


        private void FieldFrom_Load(object sender, EventArgs e)
        {

        }
    }
}
