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
using ChessLib;
using ChessLib.Enums.Game;
using ChessDiploma.Windows.UserMenuWindows;
using ChessDiploma.Windows.UserMenuWindows.GameWindows;

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

        private Player _player;
        private Player _enemy;
        private ReplayOrGame? _formType = null;

        private const int _timerWidth = 125;
        private bool _ifCellIsClicked = false;
        private const int _amountOfNmbersOnBoard = 8;

        public FieldFrom(Game game, ReplayOrGame type)
        {
            _formType = type;
            Data.InitGameFieldToRepaly(game);

            InitializeComponent();
            InitGameParams();
        }
        public FieldFrom()
        {
            _formType = ReplayOrGame.Game;
            InitializeComponent();
            InitGameParams();
        }
        public FieldFrom(Player player, Player enemy)
        {
            _player = player;
            _enemy = enemy;
            InitializeComponent();
            Data._game = new Game(player, enemy);
            InitGameParams();
        }
        public void InitGameParams()
        {
            InitEndTimerEvent();
            InitFormToCloseByTimer();

            InitSizes();
            InitGamePanels();

            InitFieldPanel();

            GetImagesPath();
            InitFieldArrSize();

            FillWhiteFiguresImagesToConvertPawn();
            FillBlackFiguresImagesToConvertPawn();

            CreateConvertPawnPanels();
            FillGameMenuPanel();
            FillMainMenuPanel();

            if (_formType == ReplayOrGame.Game && Data._game.GetTime() != -1) Data._game.StartFirstTimer();

            //Check for bot step
            BotStep();
        }
        public void FillMainMenuPanel()
        {
            Button playerInfo = new Button();
            InitMainMenuButtons(playerInfo, new Point(0, 0), "player info");
            playerInfo.Click += (sender, e) =>
            {
                ChoosePlayer playerParams = new ChoosePlayer(DbUsage.GetAllUsers());
                playerParams.ShowDialog();
            };

            Control last = _mainMenuPanel.Controls[_mainMenuPanel.Controls.Count - 1];
            Button close = new Button();
            InitMainMenuButtons(close, new Point(0, last.Location.Y + last.Height), "close game");
            close.Click += (sender, e) =>
            {
                Close();
                Data._game.ClearPlayersHitLists();
            };

            Button closeProgram = new Button();
        }
        private void InitMainMenuButtons(Button but, Point loc, string text)
        {
            but.Size = new Size(_mainMenuPanel.Width, _fieldCellSize.Item1);
            but.Location = loc;
            but.Text = text;
            but.Font = new Font("Times New Roman", 16);
            but.TextAlign = ContentAlignment.MiddleCenter;
            _mainMenuPanel.Controls.Add(but);
        }
        public void FillTimer(Panel panel, string stringTime, int playerIndex)
        {
            if (Data._game.GetTime() != -1)
            {
                Label label = new Label();

                if (Data._game.IfPlayerIsUser(playerIndex))
                {
                    ((User)Data._game.Players[playerIndex]).checkTimer = label;
                }

                //label.Size = new Size(_timerWidth, _fieldCellSize.Item1);

                Point loc = new Point(panel.Width - label.Width, panel.Size.Height - _fieldCellSize.Item1);
                label.Text = stringTime;
                label.Font = new Font("Times New Roman", 18);
                label.Location = loc;
                panel.Controls.Add(label);
            }
        }

        public void InitFieldArrSize()
        {
            _field = new PictureBox[_amountOfRows, _amountOfRows];
            _originalColors = new Color[_amountOfRows, _amountOfRows];
            int cellSizePar = _fieldCellSize.Item1;
            int heightIters = 0;
            (int, int) tempLock = _firstCellLock;
            string str;
            for (int i = 0; i < _field.GetLength(0); i++)
            {
                for (int j = 0; j < _field.GetLength(1); j++)
                {
                    _field[i, j] = new PictureBox();
                    Figure figure = Data._game.InitCord((i, j));
                    str = $"({i},{j})";
                    if (j == 0)
                    {
                        tempLock = _firstCellLock;
                        int heightMultyplier = cellSizePar * heightIters;
                        tempLock = (_firstCellLock.Item1, tempLock.Item2 + heightMultyplier);
                        AddPictureBox(GetTempColor(), tempLock, (i, j), str);
                        heightIters++;
                    }
                    else
                    {
                        tempLock = (tempLock.Item1 + cellSizePar, tempLock.Item2);
                        AddPictureBox(GetCellColor(), tempLock, (i, j), str);
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
            const int thirdPart = 3;
            const int half = 2;
            const int fourthPart = 4;
            const double fieldWidthMuliplier = 1.25;

            //Main menu params
            _mainMenuSize.Width = Width / fourthPart;
            _mainMenuSize.Height = Height;

            //InGame menu params
            _inGameMenuSize.Width = Width / fourthPart;
            _inGameMenuSize.Height = Height;
            _inGameMenuLocation.X = Width / fourthPart * thirdPart;
            _inGameMenuLocation.Y = 0;

            //field params
            int sizeParam = _fieldCellSize.Item1 * (_amountOfSquaresInField /
                _amountOfRows) + _fieldBorederSize.Item1; //420
            _fieldPanel.Size = new Size(sizeParam, sizeParam);
            _fieldPanel.Location = new Point(Width / half - sizeParam / half, Height / half - sizeParam / half);

            //first player params
            int playerPanelWidthParam = (int)(_fieldPanel.Width * fieldWidthMuliplier);
            int playerPanelHeightParam = _fieldPanel.Location.Y - _indentPlayerPanelBorder * half;

            int firstPlayerPanelYLocation = _fieldPanel.Location.Y + _fieldPanel.Size.Height + _indentPlayerPanelBorder;
            _firstPlayerPanelSize = new Size(playerPanelWidthParam, playerPanelHeightParam);
            _firstPlayerPlanelLocation = new Point(Width / half - _firstPlayerPanelSize.Width / half, firstPlayerPanelYLocation);

            //second player params
            _secondPlayerPanelSize = new Size(playerPanelWidthParam, playerPanelHeightParam);
            _secondPlanelLocation = new Point(Width / half - _firstPlayerPanelSize.Width / half, _indentPlayerPanelBorder);
        }
        //CREATING FIELD 
        //FIELD - Panel
        public void InitFieldPanel()
        {
            //Size - 8 * 8 cells + borders for numbers and letters(10px)
            _fieldPanel.BorderStyle = BorderStyle.FixedSingle;
            _fieldPanel.BackColor = _borderColor;
            _fieldPanel.AutoScroll = false;
            _fieldPanel.AllowDrop = true;
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

        string startTag, endTag;
        Cursor _cursor;
        private bool _ifDroped = false;

        public void AddPictureBox(Color color, (int, int) loc, (int x, int y) cord, string tag)
        {
            _field[cord.x, cord.y].BorderStyle = BorderStyle.FixedSingle;
            _field[cord.x, cord.y].BackColor = color;
            _field[cord.x, cord.y].Size = new Size(_fieldCellSize.Item1, _fieldCellSize.Item2);
            _field[cord.x, cord.y].Location = new Point(loc.Item1, loc.Item2);
            _field[cord.x, cord.y].BorderStyle = BorderStyle.None;
            _field[cord.x, cord.y].AllowDrop = true;
            _field[cord.x, cord.y].Tag = tag;

            _originalColors[cord.x, cord.y] = color;

            _field[cord.x, cord.y].MouseEnter += (sender, e) =>
            {
                _field[cord.x, cord.y].Cursor = Cursors.Default;

                if (Data._game.IfStepersFigIsInCell(cord) && !_ifCellIsClicked &&
                _formType == ReplayOrGame.Game)
                {
                    _field[cord.x, cord.y].BackColor = Color.Yellow;
                }

            };
            _field[cord.x, cord.y].MouseLeave += (sender, e) =>
            {
                if (!_ifCellIsClicked)
                {
                    _field[cord.x, cord.y].BackColor = _originalColors[cord.x, cord.y];
                }
            };

            _field[cord.x, cord.y].GiveFeedback += (sender, e) =>
            {
                e.UseDefaultCursors = false;
                _field[cord.x, cord.y].Cursor = _cursor;
            };



            _field[cord.x, cord.y].MouseMove += (sender, e) =>
            {
                if (!(sender is PictureBox pic)) return;
                if (pic.Image is null) return;

                /*Cursor cursor = new Cursor(((Bitmap)pic.Image).GetHicon());
                pic.Cursor = cursor;*/
            };
            _field[cord.x, cord.y].MouseDown += (sender, e) =>
            {
                //return if its replay mode
                if (_formType == ReplayOrGame.Replay) return;

                if (!(sender is PictureBox pic)) return;
                if (pic.Image is null) return;

                //if figure is steppes
                if (!IfFigureIsStepers(cord)) return;

                Cursor cursor = new Cursor(((Bitmap)pic.Image).GetHicon());
                pic.Cursor = cursor;
                _cursor = cursor;

                startTag = pic.Tag.ToString();

                //get moves, paint BGs cords to step on in yellow
                InitMoveForStepper(cord);

                if (pic.DoDragDrop(pic.Image, DragDropEffects.Move) == DragDropEffects.Move)
                {
                    if (_ifDroped)
                    {
                        (int, int) endTagCord = GetCordByTag(endTag);

                        if (endTagCord != (-1, -1) && startTag != endTag &&
                        _field[endTagCord.Item1, endTagCord.Item2].BackColor == Color.Yellow)
                        {
                            pic.Visible = false;
                            RasingMoveDragDrop(endTagCord);
                            pic.Image = null;
                            pic.Visible = true;
                        }
                    }
                }
                AssignOriginalColors();
                if (Data._game._ifGameEnded)
                {
                    MessageBox.Show(GetGameResString(), "Game Ended!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                _ifCellIsClicked = false;
            };

            _field[cord.x, cord.y].DragOver += (sender, e) =>
            {
                PictureBox pictureBox = sender as PictureBox;

                if (pictureBox != null && pictureBox.Image != null)
                {
                    e.Effect = DragDropEffects.Move;
                }
                if (_field[cord.x, cord.y].BackColor == Color.Yellow && Data._game.IfFigureIsEnemysOrEmpty(cord))
                {
                    const int diffPicLength = 2;
                    const int penSize = 1;
                    Graphics g = pictureBox.CreateGraphics();

                    // Задаем цвет и толщину рамки (границы)
                    Pen pen = new Pen(_borderColor, diffPicLength);

                    // Получаем размеры PictureBox (без учета границ)
                    int pbWidth = pictureBox.Width - diffPicLength; // -2 для учета границ
                    int pbHeight = pictureBox.Height - diffPicLength; // -2 для учета границ

                    // Рисуем рамку внутри границ PictureBox
                    g.DrawRectangle(pen, penSize, penSize, pbWidth, pbHeight);

                    // Освобождаем ресурсы
                    pen.Dispose();
                    g.Dispose();
                }
            };
            _field[cord.x, cord.y].DragLeave += (sender, e) =>
            {
                PictureBox box = sender as PictureBox;
                if (_field[cord.x, cord.y].BackColor == Color.Yellow)
                {
                    // Получаем объект Graphics для рисования на PictureBox
                    Graphics g = box.CreateGraphics();

                    // Очищаем содержимое PictureBox

                    //g.Clear(box.BackColor);

                    _field[cord.x, cord.y].Invalidate();


                    // Освобождаем ресурсы
                    g.Dispose();
                }
            };

            _field[cord.x, cord.y].DragDrop += (sender, e) =>
            {
                //if figure can step on this cell
                if (_field[cord.x, cord.y].BackColor != Color.Yellow)
                {
                    _ifDroped = false;
                    return;
                }
                Image img = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
                ((PictureBox)sender).Image = img;

                endTag = ((PictureBox)sender).Tag.ToString();
                _ifDroped = true;
            };

            _field[cord.x, cord.y].DragEnter += (sender, e) =>
            {
                PictureBox box = sender as PictureBox;

                if (e.Data.GetDataPresent(DataFormats.Bitmap))
                {
                    e.Effect = DragDropEffects.Move;
                }
            };
            _field[cord.x, cord.y].MouseLeave += (sender, e) =>
            {
                if (sender is PictureBox pic)
                {
                    pic.Cursor = Cursors.Default;
                }
            };
            _fieldPanel.MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Default;
            };

            _field[cord.x, cord.y].MouseUp += (sender, e) =>
            {
                if (_formType == ReplayOrGame.Replay) return;
                _chosenStepIndex = GetMoveIndexToMake(cord);

                CheckForDrawByWithOutHitAndEqualMove();

                if (_field[cord.x, cord.y].BackColor == Color.Yellow && _chosenStepIndex != -1 && _ifCellIsClicked)
                {
                    TotalMoveReassign(_moves.PossibleMoves[_chosenStepIndex]);
                    AddMoveInHistory(_moves.PossibleMoves[_chosenStepIndex], false);

                    if (!(_moves.PossibleMoves[_chosenStepIndex].HitFigure is null))
                    {
                        Figure fig = _moves.PossibleMoves[_chosenStepIndex].HitFigure;
                        UpdateHitFiguresPanel(fig);
                    }

                    AssignOriginalColors();

                    IfCheckMateOrPate();

                    if (Data._game.IfPawnCameToTheEndOfBoard())
                    {
                        ShowConvertPanel(cord);
                    }
                    else
                    {
                        ChangeSteper();

                        //Check for bot step
                        BotStep();
                    }
                    return;
                }
                AssignOriginalColors();
                _ifCellIsClicked = false;
                if (!(_field[cord.x, cord.y].Image is null) && Data._game.IfFigureIsStepers(cord))
                {
                    _field[cord.x, cord.y].BackColor = Color.Yellow;
                    _ifCellIsClicked = true;
                    _moves = new AllMoves();

                    _moves = Data._game.GetMovesForFigure(cord);

                    ShowEndPointsToMove();
                }
            };
            _fieldPanel.Controls.Add(_field[cord.x, cord.y]);
        }
        private void RasingMoveDragDrop((int, int) cord)
        {
            _chosenStepIndex = GetMoveIndexToMake(cord);

            TotalMoveReassign(_moves.PossibleMoves[_chosenStepIndex]);
            AddMoveInHistory(_moves.PossibleMoves[_chosenStepIndex], false);

            AssignOriginalColors();

            if (!(_moves.PossibleMoves[_chosenStepIndex].HitFigure is null))
            {
                Figure fig = _moves.PossibleMoves[_chosenStepIndex].HitFigure;
                UpdateHitFiguresPanel(fig);
            }
            CheckForDrawByWithOutHitAndEqualMove();

            if(IfCheckMateOrPate()) return;

            if (Data._game.IfPawnCameToTheEndOfBoard())
            {
                ShowConvertPanel(cord);
            }
            else
            {
                ChangeSteper();
                BotStep();
            }
        }
        private void InitMoveForStepper((int x, int y) cord)
        {
            if (!(_field[cord.x, cord.y].Image is null) && Data._game.IfFigureIsStepers(cord))
            {
                _field[cord.x, cord.y].BackColor = Color.Yellow;
                _ifCellIsClicked = true;
                _moves = new AllMoves();

                _moves = Data._game.GetMovesForFigure(cord);

                ShowEndPointsToMove();
            }
        }
        public bool IfFigureIsStepers((int x, int y) cord)
        {
            return Data._game.IfFigureIsStepers(cord);
        }
        public (int, int) GetCordByTag(string picBoxTag)
        {
            (int x, int y) cord = (-1, -1);

            int counter = 0;
            for (int i = 0; i < picBoxTag.Length; i++)
            {
                if (counter == 0 && Char.IsDigit(picBoxTag[i]))
                {
                    int.TryParse(picBoxTag[i].ToString(), out cord.x);
                    counter++;
                }
                else if (counter == 1 && Char.IsDigit(picBoxTag[i]))
                {
                    int.TryParse(picBoxTag[i].ToString(), out cord.y);
                    return cord;
                }
            }
            return cord;
        }

        private void BotStep()
        {
            if (Data._game.IfSteperIsBot())
            {
                Move move = Data._game.GetMoveForBot();
                TotalMoveReassign(move);
                AddMoveInHistory(move, false);
                IfCheckMateOrPate();
                ChangeSteper();
            }
        }
        public bool IfCheckMateOrPate()
        {
            if (Data._game.IfSteperCheckMated())
            {
                Data._game.StopTimers();
                Data._game.InitGameResult(Data._game.GetWinnerResult());
                Data._game.StepperWonTheGame();

                Data._game.InitWinnerGameResult();
                GameEnded();
                Data._game._ifGameEnded = true;
                return true;
            }
            else if (Data._game.IfGameEndedByPate())
            {
                Data._game.StopTimers();
                Data._game.GameEndedByDraw();
                Data._game.InitGameResultDraw();
                GameEnded();
                Data._game.InitGameResult(GameResult.Draw);

                Data._game._ifGameEnded = true;
                return true;
            }
            return false;
        }
        public void CheckForDrawByWithOutHitAndEqualMove()
        {
            if (Data._game.IfItsDrawByEqualMoves())
            {
                Data._game.StopTimers();
                Data._game.GameEndedByDraw();
                Data._game.InitGameResultDraw();

                GameEnded();
                //Close();
            }
            else if (Data._game.IfItsDrawByMovesWithoutHitting())
            {
                Data._game.StopTimers();
                Data._game.InitGameResult(GameResult.Draw);
                Data._game.GameEndedByDraw();
                Data._game.InitGameResultDraw();

                GameEnded();
                //Close();
            }
        }
        public void TotalMoveReassign(Move move)
        {
            //Add fig to move in history with not maken move
            Data._game.AddFigToMoveInHistory(move);
            //reassign in logic
            Data._game.ReassignMove(move);

            //reassign in form
            ReassignMove(move);
            //ReassignMove(move);
            Data._game.IfSpecialFigIsMoves(move);
        }

        public void UpdateHitFiguresPanel(Figure figure)
        {
            Player player = Data._game.GetPlayerThatHitFigure(figure);
            Data._game.AddHitFigure(figure, 1);
            InitHitFiguresPanel(GetHitFigsPanel(player), player);
        }
        public void DeleteFromHitFiguresPanel(Move move)
        {
            Player player = Data._game.GetPlayerThatHitFigure(move.HitFigure);
            Data._game.AddHitFigure(move.HitFigure, -1);

            InitHitFiguresPanel(GetHitFigsPanel(player), player);
        }

        public void AddMoveInHistory(Move move, bool ifConvertionIsHappend)
        {
            if (ifConvertionIsHappend || move.ConvertFigure is null)
            {
                AddMoveInMovesHistory(move,
                    Data._game.GetPlayerColor().ToString());
                Data._game.AddMoveInHistory(move);
            }
        }

        public Control AddMoveInMovesHistory(Move move, string steperColor)
        {
            FlowLayoutPanel movesPanel = (FlowLayoutPanel)FindFlowLayotPanel(_inGameMenu);

            string toSaveString = InitMoveInMoveList(move, steperColor);

            const int sizeWidth = 10;
            const int heightWidth = 10;

            Label newMove = new Label();
            newMove.AutoSize = true;
            newMove.Size = new Size(sizeWidth, heightWidth);
            newMove.Text = toSaveString;
            newMove.Font = new Font("Times New Roman", 14);
            movesPanel.Controls.Add(newMove);

            return newMove;
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
            const int castlingMoveAmount = 4;
            const string moveDevider = " - ";
            const string shortCastling = "O-O";
            const string largeCastling = "O-O-O";
            const string hitSign = " x ";


            if (move.OneMove.Count == castlingMoveAmount)
            {
                result = stepperColor + moveDevider;
                result += move.OneMove.Exists(x => x.Item2 == 0) ? largeCastling : shortCastling;
            }
            else if (!(move.HitFigure is null))
            {
                (int, int) from = move.OneMove.First();
                (int, int) to = move.OneMove.Last();

                result = stepperColor + moveDevider + ConvertMove(from.Item2) + ConvertNumber(from.Item1) + hitSign +
                    ConvertMove(to.Item2) + ConvertNumber(to.Item1);
            }
            else //usual move
            {
                (int, int) from = move.OneMove.First();
                (int, int) to = move.OneMove.Last();

                result = stepperColor + moveDevider + ConvertMove(from.Item2) + ConvertNumber(from.Item1) + moveDevider +
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
            return (_amountOfNmbersOnBoard - number).ToString();
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
            const int half = 2;
            const int thirdPart = 3;
            if (!_leftXConvertErrorCords.Contains(cord.x) &&
                !_rightXConvertErrorCords.Contains(cord.x))
            {
                return pointX + _fieldCellSize.Item1 / half - panelWidth / half;
            }
            else if (_leftXConvertErrorCords.Contains(cord.x))
            {
                return pointX;
            }
            else // cordX - 6 || 7
            {
                return pointX - _fieldCellSize.Item1 * thirdPart;
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
                    AddMoveInHistory(_moves.PossibleMoves[_chosenStepIndex], true);

                    HideConvertPanels();
                    ChangeSteper();
                };

                convertPanel.Controls.Add(picBox);
            }
        }
        public void ChangeSteper()
        {
            _ifCellIsClicked = false;
            Data._game.ChangeSteper();

            int inGameMenuSteperIndex = GetInGameMenuSteperIndex();
            if (inGameMenuSteperIndex != -1)
            {
                (string, string) stepperParams =
                    Data._game.GetSteppersNameAdnColor();
                _inGameMenu.Controls[inGameMenuSteperIndex].Text =
                    "Stepper:\n" + stepperParams.Item1 + ". Color - " + stepperParams.Item2;
            }

        }
        public int GetInGameMenuSteperIndex()
        {
            return (_inGameMenu.Controls.Count > 0 && _inGameMenu.Controls[0] is Label) ? 0 : -1;
        }

        public ConvertPawn GetFigureTypeToConvert(string figName)
        {
            return figName.Contains("Queen") ? ConvertPawn.Queen :
                   figName.Contains("Rook") ? ConvertPawn.Rook :
                   figName.Contains("Bishop") ? ConvertPawn.Bishop :
                   figName.Contains("Horse") ? ConvertPawn.Horse : new ConvertPawn();

            /*          if (figName.Contains("Queen")) return ConvertPawn.Queen;
                        if (figName == "WhiteRook" || figName == "BlackRook") return ConvertPawn.Rook;
                        if (figName == "WhiteBishop" || figName == "BlackBishop") return ConvertPawn.Bishop;
                        if (figName == "WhiteHorse" || figName == "BlackHorse") return ConvertPawn.Horse;
                        return new ConvertPawn();*/
        }
        public void HideConvertPanels()
        {
            _convertBlack.Hide();
            _convertWhite.Hide();
        }
        public void ReassignMove(Move move)
        {
            if (!move.IfMoveContainsTime()) Data._game.AssignTimeOnTimerInMove(move);
            Data._game.InitCastlingType(move);
            const int half = 2;

            Image toFill = null;
            for (int i = 0; i < move.OneMove.Count; i++)
            {
                (int x, int y) cord = move.OneMove[i];
                if (i % half == 0)
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
            const int pointsInUsualMove = 2;
            const int pointIndexInCastling = 1;
            for (int i = 0; i < _moves.PossibleMoves.Count; i++)
            {
                if (_moves.PossibleMoves[i].OneMove.Count == pointsInUsualMove)
                {
                    (int x, int y) endMovePoint = _moves.PossibleMoves[i].OneMove.Last();
                    _field[endMovePoint.x, endMovePoint.y].BackColor = Color.Yellow;
                }
                else
                {
                    (int x, int y) endMovePoint = _moves.PossibleMoves[i].OneMove[pointIndexInCastling];
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
            for (int i = 0; i < _field.GetLength(0); i++)
            {
                for (int j = 0; j < _field.GetLength(1); j++)
                {
                    _field[i, j].Refresh();
                }
            }



        }
        public void InitGamePanels()
        {
            InitMainMenuPanel();
            InitGameMenuPanel();

            InitPlayerPanel(_firstPlayerPanel, Data._game.GetFirstPlayerName(), "hitFigsFirst", _firstPlayerPanelSize, _firstPlayerPlanelLocation);
            InitPlayerPanel(_secondPlayerPanel, Data._game.GetLastPlayerName(), "hitFigsSecond", _secondPlayerPanelSize, _secondPlanelLocation);

            Player firstPlayer = Data._game.GetFirstPlayer();
            Player secondPlayer = Data._game.GetLastPlayer();

            FillPlayerPlanels(_firstPlayerPanel, firstPlayer);
            FillPlayerPlanels(_secondPlayerPanel, secondPlayer);

            if (_formType == ReplayOrGame.Game && Data._game.GetTime() != -1)
            {
                FillTimer(_firstPlayerPanel, Data._game.GetPlayerCurrentTime(0), 0);
                FillTimer(_secondPlayerPanel, Data._game.GetPlayerCurrentTime(1), 1);
            }
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
            //AddDefaultLabelToPanel(_mainMenuPanel, "Main Menu");

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
            hitFigures.Size = new Size(panel.Width - _timerWidth, _fieldCellSize.Item1);
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
            name.Text = player.Login + ". Color - " + player.Color.ToString();
            name.Location = new Point(0, 0);
            name.Name = player.Login;

            panel.Controls.Add(name);
        }
        public void InitHitFiguresPanel(Panel hitFigPanel, Player player)
        {
            hitFigPanel.Controls.Clear();
            int count = Data._game.GetAmountOfHitFigureList();
            PlayerColor color = player.Color;// Data._game.GetPlayerColor();

            const int panelWidth = 75;
            for (int i = 0; i < count; i++)
            {
                (FigType name, int amount) hitFig = Data._game.GetPlayerHitFigure(i, player);
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
        public Image GetHitFigPictureByName(PlayerColor tempPlayerColor, FigType type)
        {
            if (tempPlayerColor == PlayerColor.White)
            {
                return type == FigType.Pawn ? _images.Find(x => x.Tag.ToString() ==
                FiguresTypesByColors.BlackPawn.ToString()) :
                    type == FigType.Rook ? _images.Find(x => x.Tag.ToString() ==
                    FiguresTypesByColors.BlackRook.ToString()) :
                    type == FigType.Horse ? _images.Find(x => x.Tag.ToString() ==
                    FiguresTypesByColors.BlackHorse.ToString()) :
                    type == FigType.Bishop ? _images.Find(x => x.Tag.ToString() ==
                    FiguresTypesByColors.BlackBishop.ToString()) :
                    type == FigType.Queen ? _images.Find(x => x.Tag.ToString() ==
                    FiguresTypesByColors.BlackQueen.ToString()) : null;
            }
            else//Black
            {
                return type == FigType.Pawn ? _images.Find(x => x.Tag.ToString() ==
                FiguresTypesByColors.WhitePawn.ToString()) :
                    type == FigType.Rook ? _images.Find(x => x.Tag.ToString() ==
                    FiguresTypesByColors.WhiteRook.ToString()) :
                    type == FigType.Horse ? _images.Find(x => x.Tag.ToString() ==
                    FiguresTypesByColors.WhiteHorse.ToString()) :
                    type == FigType.Bishop ? _images.Find(x => x.Tag.ToString() ==
                    FiguresTypesByColors.WhiteBishop.ToString()) :

                    type == FigType.Queen ? _images.Find(x => x.Tag.ToString() ==
                    FiguresTypesByColors.WhiteQueen.ToString()) : null;
            }
        }
        public Panel GetHitFigsPanel(Player player)
        {
            Control[] controls = Controls.Find(player.Login, false);

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
        private string GetWhoStepsString(string common)
        {
            return _formType == ReplayOrGame.Replay ? "This is Replay" : common;
        }
        public void FillGameMenuPanel()
        {
            const int panelWidthError = 25;

            Label whoStepsName = new Label();
            (string, string) stepperParams =
                Data._game.GetSteppersNameAdnColor();
            whoStepsName.AutoSize = false;
            whoStepsName.Size = new Size(_inGameMenu.Width, _fieldCellSize.Item1 * 2);
            whoStepsName.Location = new Point(0, 0);
            whoStepsName.Text = GetWhoStepsString("Stepper: " + "\n" + stepperParams.Item1 + ". Color - " + stepperParams.Item2);
            whoStepsName.Font = new Font("Times New Roman", 20);
            whoStepsName.TextAlign = ContentAlignment.MiddleCenter;
            whoStepsName.Name = "whoSteps";
            _inGameMenu.Controls.Add(whoStepsName);


            Control last = _inGameMenu.Controls[_inGameMenu.Controls.Count - 1];
            if (_formType == ReplayOrGame.Game)
            {
                Button declineLastMove = new Button();
                GivePurumsToInGameMenuButtons(declineLastMove, "Decline last move",
                    new Size(_inGameMenu.Width, _fieldCellSize.Item1),
                    new Point(0, whoStepsName.Location.Y + whoStepsName.Size.Height));

                declineLastMove.Click += DeclineLastMove_Click;

                Button sendDrawOffer = new Button();
                last = _inGameMenu.Controls[_inGameMenu.Controls.Count - 1];
                GivePurumsToInGameMenuButtons(sendDrawOffer, "Send draw offer",
                    new Size(_inGameMenu.Width, _fieldCellSize.Item1),
                    new Point(0, last.Location.Y + last.Size.Height));

                sendDrawOffer.Click += SendDraw_Click;

                Button giveUp = new Button();
                last = _inGameMenu.Controls[_inGameMenu.Controls.Count - 1];
                GivePurumsToInGameMenuButtons(giveUp, "Give up",
                    new Size(_inGameMenu.Width, _fieldCellSize.Item1),
                    new Point(0, last.Location.Y + last.Size.Height));
                giveUp.Click += Givup_Click;
            }
            else//Its replay
            {
                Button previousMove = new Button();
                GivePurumsToInGameMenuButtons(previousMove, "Go to previous move",
                    new Size(_inGameMenu.Width, _fieldCellSize.Item1),
                    new Point(0, whoStepsName.Location.Y + whoStepsName.Size.Height));

                previousMove.Click += GoToPreviousMove_Click;

                Button nextMove = new Button();
                GivePurumsToInGameMenuButtons(nextMove, "Go to next Move",
                new Size(_inGameMenu.Width, _fieldCellSize.Item1),
                 new Point(0, previousMove.Location.Y + previousMove.Size.Height));

                nextMove.Click += GoToNextMove_Click;
            }

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
            movesList.Size = new Size(_inGameMenu.Width - panelWidthError, checkPanel.Height);
            movesList.FlowDirection = FlowDirection.TopDown;
            checkPanel.Controls.Add(movesList);


            if (_formType == ReplayOrGame.Replay)//add moves in flowLayoutPanel
            {
                FillMoveHistoryPanelForReplay();

                InitGameExodus(movesList);
            }
        }
        public void InitGameExodus(Panel panel)
        {
            GameResult res = Data._game.GameExodus;
            string message = res == GameResult.FirstWon ? $"Won - {Data._game.GetFirstPlayerName()}" :
                res == GameResult.SecondWon ? $"Won - {Data._game.GetSecondPlayerName()}" :
                res == GameResult.Draw ? "Its Draw" : "Game was closed";

            Label lb = new Label();
            lb.Size = new Size(panel.Width, _fieldCellSize.Item1);
            lb.Font = new Font("Times New Roman", 16);
            lb.Text = message;
            panel.Controls.Add(lb);
        }
        private void SendDraw_Click(object sender, EventArgs e)
        {
            Player player = Data._game.GetAnoutherPlayer();

            if (player is Bot)
            {
                MessageBox.Show("Decline!");
            }
            DrawOffer offer = new DrawOffer(Data._game.GetSteper());
            offer.ShowDialog();
            if (offer._answer)
            {
                Data._game.StopTimers();
                MessageBox.Show("Accepted!");
                Data._game.GameEndedByDraw();
                Data._game.InitGameResultDraw();
                //Data.UpdatePlayersInDB();
                GameEnded();
                Data._game.InitGameResult(GameResult.Draw);


               // Close();
            }
            else
            {
                MessageBox.Show("offer declined!");
            }
        }
        private void GoToPreviousMove_Click(object sender, EventArgs e)
        {
            //Check If index >= 0
            //Get history panel
            //Compare repaint(Change backColor) temp move (comparation with field index);
            //reassign in filed + in Form 

            bool check = Data._game.IfCanGetPreviousMove();
            if (!check)
            {
                MessageBox.Show("Cant get previous move", "Mistake!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int moveIndex = Data._game.GetMoveIndexForReplay();
            FlowLayoutPanel movesPanel = (FlowLayoutPanel)FindFlowLayotPanel(_inGameMenu);

            PaintMoveInPanelInReplyMode(movesPanel);
            PaintMoveInPanelInReplyMode(moveIndex, movesPanel);

            //Reassing last move
            ReassignMoveInPreviousMoveInReplayMode(moveIndex);
            Data._game.PreviousMoveForReplay();

            Move move = Data._game.GetMoveFromHistory(moveIndex);

            if (!(move.HitFigure is null))
            {
                DeleteFromHitFiguresPanel(move);
            }
        }
        public void ReassignMoveInPreviousMoveInReplayMode(int moveIndex)
        {
            Move move = Data._game.GetMoveFromHistory(moveIndex);

            //reassign in logic
            Data._game.DeclineMove(move);
            //reassign in field
            GoBackMoveInReplayMode(move);
        }
        private void GoBackMoveInReplayMode(Move move)
        {
            const int pointInUsualMove = 2;
            const int firstFigtempPointIndex = 0;
            const int firstFigStepOnPointIndex = 1;
            const int secondFigtempPointIndex = 2;
            const int secondFigStepOnPointIndex = 3;

            Move convertedMove = new Move();
            if (move.OneMove.Count > pointInUsualMove)
            {
                convertedMove.OneMove = new List<(int, int)>()
                { move.OneMove[firstFigStepOnPointIndex], move.OneMove[firstFigtempPointIndex],
                    move.OneMove[secondFigStepOnPointIndex], move.OneMove[secondFigtempPointIndex] };
                ReassignMove(convertedMove);
                return;
            }

            (int, int) from = move.OneMove.First();
            (int, int) to = move.OneMove.Last();

            _field[from.Item1, from.Item2].Image = _field[to.Item1, to.Item2].Image;
            _field[to.Item1, to.Item2].Image = null;


            if (move.ConvertFigure != null)
            {
                _field[from.Item1, from.Item2].Image = GetPawnImage(move.GetPlayerColor());
                _field[to.Item1, to.Item2].Image = null;
            }
            if (move.HitFigure != null)
            {
                _field[to.Item1, to.Item2].Image = GetHitFigureImage(move.HitFigure);
            }

        }

        private void GoToNextMove_Click(object sender, EventArgs e)
        {
            //Check If its last move
            //Get histry panel 
            //Compare repaint(Change backColor) temp move (comparation with field index);
            //reassign in filed + in Form 

            if (!Data._game.IfCanGetNextMove())
            {
                MessageBox.Show("Cant get next move", "Mistake!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Data._game.NextMoveForReplay();
            int moveIndex = Data._game.GetMoveIndexForReplay();

            FlowLayoutPanel movesPanel = (FlowLayoutPanel)FindFlowLayotPanel(_inGameMenu);

            PaintMoveInPanelInReplyMode(movesPanel);

            PaintMoveInPanelInReplyMode(moveIndex, movesPanel);

            //reassign move in field + form
            ReassignMoveInNextMoveInReplayMode(moveIndex);


            if (!(Data._game.GetMoveFromHistory(moveIndex).HitFigure is null))
            {
                Figure fig = Data._game.GetMoveFromHistory(moveIndex).HitFigure;
                UpdateHitFiguresPanel(fig);
            }
        }
        private void FillMoveHistoryPanelForReplay()
        {
            List<Move> moves = Data._game.GetMoveHistory();

            for (int i = 0; i < moves.Count; i++)
            {
                _ = AddMoveInMovesHistory(moves[i],
                moves[i].GetPlayerColor().ToString());
            }
        }
        public void ReassignMoveInNextMoveInReplayMode(int moveIndex)
        {
            //get move to reassing
            //reassign it in field
            //reassign it in logic

            Move move = Data._game.GetMoveForReplay();

            ReassignMove(move);
            if (move.ConvertFigure != null)
            {
                ConvertFigureInNextMoveInReplayer(move);
            }

            Data._game.AddMovedFigureInDisplayMode(move, moveIndex);
            Data._game.ReassignMove(move);
        }
        public void ConvertFigureInNextMoveInReplayer(Move move)
        {
            Image img = GetConvertImage((ConvertPawn)move.ConvertFigure, move.GetPlayerColor());

            (int, int) cord = move.OneMove.Last();

            _field[cord.Item1, cord.Item2].Image = img;
        }
        private Image GetConvertImage(ConvertPawn type, PlayerColor steperColor)
        {

            return
            type == ConvertPawn.Rook && steperColor == PlayerColor.White ?
            _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhiteRook.ToString()) :
            type == ConvertPawn.Rook && steperColor == PlayerColor.Black ?
            _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackRook.ToString()) :

            type == ConvertPawn.Horse && steperColor == PlayerColor.White ?
            _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhiteHorse.ToString()) :
            type == ConvertPawn.Horse && steperColor == PlayerColor.Black ?
            _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackHorse.ToString()) :

            type == ConvertPawn.Bishop && steperColor == PlayerColor.White ?
            _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhiteBishop.ToString()) :
            type == ConvertPawn.Bishop && steperColor == PlayerColor.Black ?
            _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackBishop.ToString()) :

            type == ConvertPawn.Queen && steperColor == PlayerColor.White ?
            _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhiteQueen.ToString()) :
            type == ConvertPawn.Queen && steperColor == PlayerColor.Black ?
            _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackQueen.ToString()) : null;
        }

        private void PaintMoveInPanelInReplyMode(Panel panel)
        {
            for (int i = 0; i < panel.Controls.Count; i++)
            {
                panel.Controls[i].BackColor = Color.Transparent;
            }
        }
        private void PaintMoveInPanelInReplyMode(int index, Panel panel)
        {
            for (int i = 0; i < panel.Controls.Count; i++)
            {
                if (i == index)
                {
                    panel.Controls[i].BackColor = Color.Yellow;
                }
            }
        }
        private void Givup_Click(object sender, EventArgs e)
        {
            Data._game.StopTimers();
            /*            Player winer = Data._game.GetAnoutherPlayer();

                        MessageBox.Show(winer.Login + " won", "Game ended!", MessageBoxButtons.OK);*/


            Data._game.InitResultSteperGaveUp();
            Data._game.UpdetePlayersWhenSteperGaveUp();

            GameEnded();

            Data._game.InitGameResult(Data._game.GetOpositWinner());

            //Close();
        }

        public void GameEnded()
        {
            Data.InitGamesEndDate();

            InitRaintingAfterGamedEnded();

            Data.InitGameInDB();

            Data._game._ifGameEnded = true;
        }
        public void InitRaintingAfterGamedEnded()
        {
            (int, int) res = Data._game.CalcPlayersRainting();

            Data._game.InitResScore(res);

/*            if (res != (-1, -1))
            {
                string text = Data._game.GetFirstPlayerName() + " earned - " + res.Item1.ToString() + "\n" +
                    Data._game.GetSecondPlayerName() + " earned - " + res.Item2.ToString();

                MessageBox.Show(text, "Game ended!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }*/
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
                MessageBox.Show("Nothing to decline!", "Mistake!");
                return;
            }
            //Get last move 
            Move lastMove = Data._game.GetLastMove();
            //reassign it in logic 
            Data._game.DeclineMove(lastMove);
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
            if (!(lastMove.HitFigure is null))
            {
                DeleteFromHitFiguresPanel(lastMove);
            }
        }
        private void DeclineLastMove(Move move)
        {
            //castling
            const int pointInUsualMove = 2;
            const int firstFigtempPointIndex = 0;
            const int firstFigStepOnPointIndex = 1;
            const int secondFigtempPointIndex = 2;
            const int secondFigStepOnPointIndex = 3;
            if (move.OneMove.Count > pointInUsualMove)
            {
                Move backMove = new Move(new List<(int, int)>()
                { move.OneMove[firstFigStepOnPointIndex], move.OneMove[firstFigtempPointIndex],
                    move.OneMove[secondFigStepOnPointIndex], move.OneMove[secondFigtempPointIndex] });
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
                return figure.FigureColor == PlayerColor.White ?
                    _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhitePawn.ToString()) :
                       figure.FigureColor == PlayerColor.Black ?
                       _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackPawn.ToString()) :
                       null;
            }
            else if (figure is Rook)
            {
                return figure.FigureColor == PlayerColor.White ?
                    _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhiteRook.ToString()) :
                       figure.FigureColor == PlayerColor.Black ?
                       _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackRook.ToString()) :
                       null;
            }
            else if (figure is Horse)
            {
                return figure.FigureColor == PlayerColor.White ?
                    _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhiteHorse.ToString()) :
                       figure.FigureColor == PlayerColor.Black ?
                       _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackHorse.ToString()) :
                       null;
            }
            else if (figure is Bishop)
            {
                return figure.FigureColor == PlayerColor.White ?
                    _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhiteBishop.ToString()) :
                       figure.FigureColor == PlayerColor.Black ?
                       _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackBishop.ToString()) :
                       null;
            }
            else if (figure is Queen)
            {
                return figure.FigureColor == PlayerColor.White ?
                    _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.WhiteQueen.ToString()) :
                       figure.FigureColor == PlayerColor.Black ?
                       _images.Find(x => x.Tag.ToString() == FiguresTypesByColors.BlackQueen.ToString()) :
                       null;
            }
            return null;
        }
        public Image GetPawnImage(PlayerColor color)
        {
            string imgTag = GetPawnTypeColor(color);
            return color == PlayerColor.White ?
                _images.Find(x => x.Tag.ToString() == imgTag) :
                _images.Find(x => x.Tag.ToString() == imgTag);
        }
        private string GetPawnTypeColor(PlayerColor color)
        {
            return color == PlayerColor.White ? FiguresTypesByColors.WhitePawn.ToString() :
                FiguresTypesByColors.BlackPawn.ToString();
        }
        private void FieldFrom_Load(object sender, EventArgs e)
        {

        }
        private void FieldFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            Data._game.StopTimers();
        }
        public delegate void CloseFormDelegate(Form form);

        private static void Timer_TimerFinished(object sender, EventArgs e, Form form)
        {
            Player winer = Data._game.GetAnoutherPlayer();
            //MessageBox.Show(winer.Login + " won", "Game ended!", MessageBoxButtons.OK);
            Data._game.InitResultSteperGaveUp();
            Data._game.UpdetePlayersWhenSteperGaveUp();

            Data.InitGamesEndDate();
            Data.InitGameInDB();
            form.Close();
        }
        private void InitEndTimerEvent()
        {
            List<Player> players = Data._game.GetPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                if (Data._game.IfPlayerIsUser(i))
                {
                    ((User)players[i]).TimerFinished += Timer_TimerFinished;
                }
            }
        }
        private void InitFormToCloseByTimer()
        {
            List<Player> players = Data._game.GetPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                if (Data._game.IfPlayerIsUser(i))
                {
                    ((User)players[i]).GetFormToCloseByTimer(this);
                }
            }
        }

        private string GetGameResString()
        {
            (int, int) scoreRes = Data._game.GetResScore();

            string scoreStr = "\nFirst got = " + scoreRes.Item1 + "\nSecond got = " + scoreRes.Item2;

            if (Data._game._gameResult == GameResult.FirstWon)
            {
                return "first player won" + scoreStr;
            }
            else if (Data._game._gameResult == GameResult.SecondWon)
            {
                return "second player won" + scoreStr;
            }
            else if (Data._game._gameResult == GameResult.Draw)
            {
                return "Its draw!" + scoreStr;
            }
            return "";
        }
    }
}
