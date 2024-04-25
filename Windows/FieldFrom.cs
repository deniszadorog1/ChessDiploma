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
        private List<int> _leftXConvertErrorCords = new List<int>() { 0, 1 };
        private List<int> _rightXConvertErrorCords = new List<int>() { 6, 7 };

        private List<char> _lettersToSave = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        private int _chosenStepIndex = -1;
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
            FillGameMenuPanel();
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
                _chosenStepIndex = GetMoveIndexToMake(cord);

                if (_field[cord.x, cord.y].BackColor == Color.Yellow && _chosenStepIndex != -1)
                {
                    //Add fig to move in history with not maken move
                    Data._game.AddFigToMoveInHistory(_moves.PossibleMoves[_chosenStepIndex]);
                    //reassign in logic
                    Data._game.ReassignMove(_moves.PossibleMoves[_chosenStepIndex]);

                    //reassign in form
                    ReassignMove(_moves.PossibleMoves[_chosenStepIndex]);
                    Data._game.IfSpecialFigIsMoves(_moves.PossibleMoves[_chosenStepIndex]);

                    AddMoveInHistory(_chosenStepIndex, false);

                    if (!(_moves.PossibleMoves[_chosenStepIndex].HitFigure is null))
                    {
                        UpdateHitFiguresPanel();
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
                        ChangeSteper();
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

        public void UpdateHitFiguresPanel()
        {
            Figure fig = _moves.PossibleMoves[_chosenStepIndex].HitFigure;
            Data._game.AddHitFigure(fig, 1);
            InitHitFiguresPanel(GetHitFigsPanel());
        }
        public void DeleteFromHitFiguresPanel(Move move)
        {
            Data._game.AddHitFigure(move.HitFigure, -1);
            InitHitFiguresPanel(GetHitFigsPanel());
        }

        public void AddMoveInHistory(int moveIndex, bool ifConvertionIsHappend)
        {
            if (ifConvertionIsHappend || _moves.PossibleMoves[moveIndex].ConvertFigure is null)
            {
                AddMoveInMovesHistory(_moves.PossibleMoves[moveIndex],
                    Data._game.GetPlayerColor().ToString());
                Data._game.AddMoveInHistory(_moves.PossibleMoves[moveIndex]);
            }
        }

        public void AddMoveInMovesHistory(Move move, string steperColor)
        {
            FlowLayoutPanel movesPanel = (FlowLayoutPanel)FindFlowLayotPanel(_inGameMenu);

            string toSaveString = InitMoveInMoveList(move, steperColor);

            Label newMove = new Label();
            newMove.AutoSize = true;
            newMove.Size = new Size(10, 50);
            newMove.Text = toSaveString;
            newMove.Font = new Font("Times New Roman", 14);
            movesPanel.Controls.Add(newMove);

            //MessageBox.Show("asd");
        }
        public void RemoveLastMove()
        {
            FlowLayoutPanel movesPanel = (FlowLayoutPanel)FindFlowLayotPanel(_inGameMenu);

            movesPanel.Controls.RemoveAt(movesPanel.Controls.Count - 1);
        }
        public Control FindFlowLayotPanel(Control control)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                if (control.Controls[i] is FlowLayoutPanel)
                {
                    return control.Controls[i];
                }
                if (control.Controls.Count > 0)
                {
                    Control res = FindFlowLayotPanel(control.Controls[i]);
                    if (res != null)
                    {
                        return res;
                    }
                }
            }
            return null;
        }
        public string InitMoveInMoveList(Move move, string stepperColor)
        {
            //Get move 
            //take move in to groups(From - To) (if just step)
            //castling - 0-0 - long castling
            //castling - 0-0-0 - short castling
            //pawn converting {from}-{ConvertedInFigure} {Q,R,H,B}
            //Hiiting {From}x{To}

            string result = "";
            int castlingMoveAmount = 4;
            if (move.OneMove.Count == castlingMoveAmount)
            {
                result = stepperColor + " - ";
                result += move.OneMove.Exists(x => x.Item2 == 0) ? "O-O-O" : "O-O";
            }
            else if (!(move.HitFigure is null))
            {
                (int, int) from = move.OneMove.First();
                (int, int) to = move.OneMove.Last();

                result = stepperColor + " - " + ConvertMove(from.Item2) + ConvertNumber(from.Item1) + " x " +
                    ConvertMove(to.Item2) + ConvertNumber(to.Item1);
            }
            else //usual move
            {
                (int, int) from = move.OneMove.First();
                (int, int) to = move.OneMove.Last();

                result = stepperColor + " - " + ConvertMove(from.Item2) + ConvertNumber(from.Item1) + " - " +
                    ConvertMove(to.Item2) + ConvertNumber(to.Item1);
            }
            return result;
        }
        public char ConvertMove(int number)
        {
            return _lettersToSave[number];
        }
        public string ConvertNumber(int number)
        {
            return (8 - number).ToString();
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
            _convertBlack.Hide();
            _convertWhite.Hide();
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

                    ConvertPawn convert = GetFigureTypeToConvert(convertedImg.Tag.ToString());
                    Data._game.ConvertFigure(figCordToConvert, convert);

                    Data._game.ClearConvertationVariable();

                    _moves.PossibleMoves[_chosenStepIndex].ConvertFigure = convert;
                    AddMoveInHistory(_chosenStepIndex, true);

                    HideConvertPanels();
                    ChangeSteper();
                };

                convertPanel.Controls.Add(picBox);
            }
        }
        public void ChangeSteper()
        {
            Data._game.ChangeSteper();

            int inGameMenuSteperIndex = GetInGameMenuSteperIndex();
            if (inGameMenuSteperIndex != -1)
            {
                (string, string) stepperParams =
                    Data._game.GetSteppersNameAdnColor();
                _inGameMenu.Controls[inGameMenuSteperIndex].Text =
                    "Stepper: " + "\n" + stepperParams.Item1 + ". Color - " + stepperParams.Item2;
            }

        }
        public int GetInGameMenuSteperIndex()
        {
            for (int i = 0; i < _inGameMenu.Controls.Count; i++)
            {
                if (i == 0 && _inGameMenu.Controls[i] is Label)
                {
                    return i;
                }
            }
            return -1;
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

            Player firstPlayer = Data._game.GetPlayer(0);
            Player secondPlayer = Data._game.GetPlayer(Data._game.Players.Count - 1);

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
            //AddDefaultLabelToPanel(_inGameMenu, "InGame Menu");

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
        public void UpdateInGameMenuWhoSteps()
        {
            if (_inGameMenu.Controls.ContainsKey("whoSteps"))
            {
                (string, string) stepperParams =
                        Data._game.GetSteppersNameAdnColor();
                _inGameMenu.Controls["whoSteps"].Text =
                    "Stepper: " + "\n" + stepperParams.Item1 + ". Color - " + stepperParams.Item2;
            }
        }
        public void FillGameMenuPanel()
        {
            Label whoStepsName = new Label();
            (string, string) stepperParams =
                Data._game.GetSteppersNameAdnColor();
            whoStepsName.AutoSize = false;
            whoStepsName.Size = new Size(_inGameMenu.Width, _fieldCellSize.Item1 * 2);
            whoStepsName.Location = new Point(0, 0);
            whoStepsName.Text = "Stepper: " + "\n" + stepperParams.Item1 + ". Color - " + stepperParams.Item2;
            whoStepsName.Font = new Font("Times New Roman", 20);
            whoStepsName.TextAlign = ContentAlignment.MiddleCenter;
            whoStepsName.Name = "whoSteps";
            _inGameMenu.Controls.Add(whoStepsName);


            Button declineLastMove = new Button();
            GivePurumsToInGameMenuButtons(declineLastMove, "Decline last move",
                new Size(_inGameMenu.Width, _fieldCellSize.Item1),
                new Point(0, whoStepsName.Location.Y + whoStepsName.Size.Height));

            declineLastMove.Click += DeclineLastMove_Click;

            Button sendDrawOffer = new Button();
            Control last = _inGameMenu.Controls[_inGameMenu.Controls.Count - 1];
            GivePurumsToInGameMenuButtons(sendDrawOffer, "Send draw offer",
                new Size(_inGameMenu.Width, _fieldCellSize.Item1),
                new Point(0, last.Location.Y + last.Size.Height));

            Button giveUp = new Button();
            last = _inGameMenu.Controls[_inGameMenu.Controls.Count - 1];
            GivePurumsToInGameMenuButtons(giveUp, "Give up",
                new Size(_inGameMenu.Width, _fieldCellSize.Item1),
                new Point(0, last.Location.Y + last.Size.Height));

            last = _inGameMenu.Controls[_inGameMenu.Controls.Count - 1];
            Label movesHist = new Label();
            movesHist.AutoSize = false;
            movesHist.Text = "Moves history";
            movesHist.Size = new Size(_inGameMenu.Width, _fieldCellSize.Item1);
            movesHist.Location = new Point(0, last.Location.Y + last.Size.Height);
            movesHist.Font = new Font("Times New Roman", 20);
            movesHist.TextAlign = ContentAlignment.BottomLeft;
            _inGameMenu.Controls.Add(movesHist);


            //-----------------
            last = _inGameMenu.Controls[_inGameMenu.Controls.Count - 1];
            Panel checkPanel = new Panel();
            checkPanel.Name = "MoveHistoryContainer";
            checkPanel.AutoScroll = true;
            checkPanel.Location = new Point(0, last.Location.Y + last.Size.Height);
            checkPanel.Size = new Size(_inGameMenu.Width, GetInGameMovesHistPanelHeight() - 15);
            checkPanel.BorderStyle = BorderStyle.FixedSingle;
            _inGameMenu.Controls.Add(checkPanel);

            FlowLayoutPanel movesList = new FlowLayoutPanel();
            movesList.Name = "moveHistory";
            movesList.BorderStyle = BorderStyle.FixedSingle;
            movesList.AutoSize = true;
            movesList.Location = new Point(0, 0);
            movesList.Size = new Size(_inGameMenu.Width - 25, checkPanel.Height);
            movesList.FlowDirection = FlowDirection.TopDown;
            checkPanel.Controls.Add(movesList);


        }
        public void AddMoveToMoveHist(string move)
        {
            Label newMove = new Label();
            newMove.AutoSize = false;
            newMove.Size = new Size(_inGameMenu.Width, _firstCellLock.Item2 * 2);
            newMove.Text = move;
            newMove.Font = new Font("Times New Roman", 14);

            int panelHistIndex = GetMoveHistoryPanelIndex();
            if (panelHistIndex != -1)
            {
                _inGameMenu.Controls[panelHistIndex].Controls.Add(newMove);
            }
        }
        public int GetMoveHistoryPanelIndex()
        {
            for (int i = 0; i < _inGameMenu.Controls.Count; i++)
            {
                if (_inGameMenu.Controls[i] is FlowLayoutPanel)
                {
                    return i;
                }
            }
            return -1;
        }
        public int GetInGameMovesHistPanelHeight()
        {
            int res = _inGameMenu.Height;
            for (int i = 0; i < _inGameMenu.Controls.Count; i++)
            {
                if (!(_inGameMenu.Controls[i] is FlowLayoutPanel))
                {
                    res -= _inGameMenu.Controls[i].Height;
                }
            }
            return res;
        }
        private void GivePurumsToInGameMenuButtons(Button but, string text, Size size, Point loc)
        {
            but.AutoSize = false;
            but.Size = size;
            but.Location = loc;
            but.Text = text;
            but.TextAlign = ContentAlignment.MiddleCenter;
            but.Font = new Font("Times New Roman", 16);
            _inGameMenu.Controls.Add(but);
        }
        private void DeclineLastMove_Click(object secnder, EventArgs e)
        {
            AssignOriginalColors();
            HideConvertPanels();

            if (!Data._game.IfCanDeclineMove())
            {
                MessageBox.Show("Cant be declined!", "Mistake!");
                return;
            }
            //Get last move 
            Move lastMove = Data._game.GetLastMove();
            //reassign it in logic 
            Data._game.DeclineLastMove();
            //reassign in field form
            DeclineLastMove(lastMove);

            //delete last in list 
            Data._game.DeleteLastMoveInHistory();
            //delete ininGame menu 
            RemoveLastMove();
            //Delete fig to move from history
            Data._game.DeleteFigToMove();
            ChangeSteper();
            //Delete from Hit In Player Panel
            DeleteFromHitFiguresPanel(lastMove);
        }
        private void DeclineLastMove(Move move)
        {
            //castling
            if (move.OneMove.Count > 2)
            {
                Move backMove = new Move(new List<(int, int)>()
                { move.OneMove[1], move.OneMove[0], move.OneMove[3], move.OneMove[2] });
                ReassignMove(backMove);
                return;
            }
            (int, int) from = move.OneMove.First();
            (int, int) to = move.OneMove.Last();
            //if figure was converted
            if (!(move.ConvertFigure is null))
            {
                //(from) - its already checnged in logic
                _field[from.Item1, from.Item2].Image = GetPawnImage(Data._game.GetFigureColor(from));
                _field[to.Item1, to.Item2].Image = null;
            }
            else
            {
                //move figure back
                _field[from.Item1, from.Item2].Image = _field[to.Item1, to.Item2].Image;
                _field[to.Item1, to.Item2].Image = null;
            }
            //if figure hit another figure
            if (!(move.HitFigure is null))
            {
                _field[to.Item1, to.Item2].Image = GetHitFigureImage(move.HitFigure);
            }
        }
        public Image GetHitFigureImage(Figure figure)
        {

            if (figure is Pawn)
            {
                return figure.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhitePawn") :
                       figure.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackPawn") :
                       null;
            }
            else if (figure is Rook)
            {
                return figure.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteRook") :
                       figure.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackRook") :
                       null;
            }
            else if (figure is Horse)
            {
                return figure.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteHorse") :
                       figure.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackHorse") :
                       null;
            }
            else if (figure is Bishop)
            {
                return figure.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteBishop") :
                       figure.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackBishop") :
                       null;
            }
            else if (figure is Queen)
            {
                return figure.FigureColor == PlayerColor.White ? _images.Find(x => x.Tag.ToString() == "WhiteQueen") :
                       figure.FigureColor == PlayerColor.Black ? _images.Find(x => x.Tag.ToString() == "BlackQueen") :
                       null;
            }
            return null;

        }
        public Image GetPawnImage(PlayerColor color)
        {
            return color == PlayerColor.White ?
                _images.Find(x => x.Tag.ToString() == "WhitePawn") :
                _images.Find(x => x.Tag.ToString() == "BlackPawn");
        }

        private void FieldFrom_Load(object sender, EventArgs e)
        {

        }
    }
}
