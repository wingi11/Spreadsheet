using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Spreadsheet
{
    public class SpreadSheet : Canvas
    {
        private readonly List<Cell> _cells = new List<Cell>();
        private readonly List<Cell> _cellsLeft = new List<Cell>();

        private readonly List<Cell> _cellsTop = new List<Cell>();

        private int _cols;
        private readonly int _colW = 90;

        private readonly int _rowH = 25;
        private int _rows;

        private Cell _selectedCell;

        private readonly List<Cell> _selectedCells = new List<Cell>();

        private bool _selecting;

        private int _selX;
        private int _selY;

        private Rectangle _bg;

        private Brush _color;

        private readonly Rectangle _selection;

        public bool Editing { get; private set; }

        public int Cols
        {
            get => _cols;
            set
            {
                Width = value * _colW;
                _cells.Clear();
                _cellsTop.Clear();
                _cellsLeft.Clear();
                Children.Clear();
                _cols = value;
                build();
            }
        }

        public int Rows
        {
            get => _rows;
            set
            {
                Height = value * _rowH;
                _cells.Clear();
                _cellsTop.Clear();
                _cellsLeft.Clear();
                Children.Clear();
                _rows = value;
                build();
            }
        }

        /// <summary>
        /// Constructor for the Spreadsheet
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        public SpreadSheet(int cols, int rows)
        {
            Mouse.Capture(this);
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Width = cols * _colW;
            Height = rows * _rowH;
            _cols = cols;
            _rows = rows;

            build();
            PreviewMouseLeftButtonDown += CellClicked;
            PreviewMouseMove += CellSelected;
            PreviewMouseLeftButtonUp += CellReleased;
            EventManager.RegisterClassHandler(typeof(Window),
                Keyboard.KeyDownEvent, new KeyEventHandler(KeyClicked), true);

            _selection = new Rectangle();
            _selection.Width = _colW;
            _selection.Height = _rowH;
            _selection.Stroke = Brushes.DodgerBlue;
            _selection.StrokeThickness = 2;
            SetLeft(_selection, 0);
            SetTop(_selection, 0);
            Children.Add(_selection);
        }

        /// <summary>
        /// Builds the Spreadsheet
        /// </summary>
        private void build()
        {
            _selX = 0;
            _selY = 0;

            _color = (SolidColorBrush) new BrushConverter().ConvertFrom("#f2f2f2");

            _bg = new Rectangle();
            _bg.Width = Width;
            _bg.Height = Height;
            _bg.Focusable = true;
            Brush bgColor = Brushes.White;
            _bg.Focus();
            _bg.Fill = bgColor;
            SetLeft(_bg, 0);
            SetTop(_bg, 0);
            Children.Add(_bg);


            Rectangle topBar = new Rectangle();
            topBar.Fill = _color;
            topBar.Stroke = null;
            topBar.Width = _cols * _colW + 20;
            topBar.Height = 20;
            SetLeft(topBar, -20);
            SetTop(topBar, -20);
            Children.Add(topBar);

            Rectangle leftBar = new Rectangle();
            leftBar.Fill = _color;
            leftBar.Stroke = null;
            leftBar.Width = 20;
            leftBar.Height = _rowH * _rows;
            SetLeft(leftBar, -20);
            SetTop(leftBar, 0);
            Children.Add(leftBar);

            LinearGradientBrush topColor = new LinearGradientBrush();
            topColor.StartPoint = new Point(0.5, 0.0);
            topColor.EndPoint = new Point(0.5, 1.0);
            topColor.GradientStops.Add(new GradientStop(Colors.DarkGray, 1));
            topColor.GradientStops.Add(new GradientStop(Color.FromArgb(255, 230, 230, 230), 0.0));

            LinearGradientBrush leftColor = new LinearGradientBrush();
            leftColor.StartPoint = new Point(0, 0.5);
            leftColor.EndPoint = new Point(1, 0.5);
            leftColor.GradientStops.Add(new GradientStop(Colors.DarkGray, 1));
            leftColor.GradientStops.Add(new GradientStop(Color.FromArgb(255, 230, 230, 230), 0.0));

            for (int x = 0; x < _cols; x++)
            {
                Line line = new Line();
                line.X1 = x * _colW;
                line.X2 = x * _colW;
                line.Y1 = 0;
                line.Y2 = _rows * _rowH;
                if (x == 0)
                    line.Stroke = Brushes.DarkGray;
                else
                    line.Stroke = (SolidColorBrush) new BrushConverter().ConvertFrom("#cecece");
                Children.Add(line);

                Line topLine = new Line();
                topLine.X1 = x * _colW;
                topLine.X2 = x * _colW;
                topLine.Y1 = -20;
                topLine.Y2 = 0;
                topLine.Stroke = topColor;
                Children.Add(topLine);

                TextBlock topText = new TextBlock();
                topText.Text = Convert.ToString(x + 1);
                SetLeft(topText, _colW * x + 3);
                SetTop(topText, -18);
                Children.Add(topText);
            }

            for (int y = 0; y < _rows; y++)
            {
                Line line = new Line();
                line.X1 = 0;
                line.X2 = _cols * _colW;
                line.Y1 = y * _rowH;
                line.Y2 = y * _rowH;
                if (y == 0)
                    line.Stroke = Brushes.DarkGray;
                else
                    line.Stroke = (SolidColorBrush) new BrushConverter().ConvertFrom("#cecece");
                Children.Add(line);

                Line leftLine = new Line();
                leftLine.X1 = 0;
                leftLine.X2 = -20;
                leftLine.Y1 = y * _rowH;
                leftLine.Y2 = y * _rowH;
                leftLine.Stroke = leftColor;
                Children.Add(leftLine);

                TextBlock leftText = new TextBlock();
                leftText.Text = Convert.ToString(y + 1);
                SetLeft(leftText, -15);
                SetTop(leftText, _rowH * y + 3);
                Children.Add(leftText);
            }
        }

        /// <summary>
        /// Selects the clicked cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellClicked(object sender, MouseButtonEventArgs e)
        {
            _selecting = true;
            ClearSelectedCells();
            if (_selectedCells.Count != 0) UpdateSelection((int) _selection.Width, (int) _selection.Height, _colW, _rowH);

            if (Editing)
                if ((int) (e.GetPosition(this).X / _colW) + (int) (e.GetPosition(this).Y / _rowH) * _cols <
                    _rows * _cols)
                {
                    Cell c = GetCell((int) (e.GetPosition(this).X / _colW),
                        (int) (e.GetPosition(this).Y / _rowH));
                    if (c != null && !c.Edit)
                    {
                        GetCell(_selX, _selY).Edit = false;
                        Editing = false;
                    }

                    if (c == null)
                    {
                        GetCell(_selX, _selY).Edit = false;
                        Editing = false;
                    }
                }

            int newSelX = (int) (e.GetPosition(this).X / _colW);
            int newSely = (int) (e.GetPosition(this).Y / _rowH);

            MoveSelection(_selX, _selY, newSelX, newSely);
            _selX = newSelX;
            _selY = newSely;

            if (e.ClickCount == 2)
            {
                Cell c = GetCell(_selX, _selY);
                if (c == null)
                {
                    Cell cell = AddCell(_selX, _selY);
                    cell.Edit = true;
                }
                else
                {
                    c.Edit = true;
                }

                Editing = true;
            }

            _selectedCell = GetCell(_selX, _selY);
        }

        /// <summary>
        /// Moves or deletes the selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyClicked(object sender, KeyEventArgs e)
        {
            int newSelX = _selX;
            int newSelY = _selY;

            Cell selected = GetCell(_selX, _selY);

            if (e.Key == Key.Enter)
            {
                selected.Selected = false;
                if (Editing) selected.Edit = false;
                if (_selY > -1 && _selY < _rows - 1) newSelY += 1;
                MoveSelection(_selX, _selY, newSelX, newSelY);
                _selY = newSelY;

                selected.Selected = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (Editing) selected.Edit = false;
            }
            else if (!Editing)
            {
                if (e.Key == Key.Right && _selX > -1 && _selX < _cols - 1) newSelX += 1;
                if (e.Key == Key.Left && _selX > 0 && _selX < _cols - 1) newSelX -= 1;

                if (e.Key == Key.Up && _selY > 0 && _selY <= _rows - 1) newSelY -= 1;
                if (e.Key == Key.Down && _selY > -1 && _selY < _rows - 1) newSelY += 1;

                MoveSelection(_selX, _selY, newSelX, newSelY);
                _selX = newSelX;
                _selY = newSelY;

                _selectedCell = GetCell(_selX, _selY);

                if (e.Key != Key.Right && e.Key != Key.Left && e.Key != Key.Up && e.Key != Key.Down &&
                    e.Key != Key.Enter && e.Key != Key.Tab && e.Key != Key.Escape)
                {
                    if (e.Key == Key.Delete && _selectedCells.Count != 0)
                    {
                        foreach (Cell cell in _selectedCells) cell.Text = "";
                    }
                    else if (e.Key == Key.Delete)
                    {
                        _selectedCell.Text = "";
                    }
                    else
                    {
                        _selectedCell.Edit = true;
                        Editing = true;
                    }
                }
                else
                {
                    ClearSelectedCells();
                }
            }
        }

        /// <summary>
        /// For multiple selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellSelected(object sender, MouseEventArgs e)
        {
            if (_selecting)
            {
            }
        }

        /// <summary>
        /// For multiple selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellReleased(object sender, MouseButtonEventArgs e)
        {
            _selecting = false;
        }

        /// <summary>
        /// For multiple selection
        /// </summary>
        private void ClearSelectedCells()
        {
            if (_selectedCells.Count != 0)
            {
                foreach (Cell cell in _selectedCells) cell.Selected = false;
                _selectedCells.Clear();
            }
        }

        /// <summary>
        /// Adds a new cell
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private Cell AddCell(int col, int row)
        {
            Cell c = new Cell(col, row, this);
            c.Width = _colW;
            c.Height = _rowH;
            _cells.Add(c);
            return c;
        }

        /// <summary>
        /// Selects a cell
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public void Select(int col, int row)
        {
            Cell selected = GetCell(col, row);
            selected.Selected = false;
            MoveSelection(_selX * _colW, _selY * _rowH, col * _colW, row * _rowH);
            _selX = col;
            _selY = row;
            selected.Selected = true;
        }

        /// <summary>
        /// Move animation for the selection rectangle
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        private void MoveSelection(int x0, int y0, int x1, int y1)
        {
            TranslateTransform transform = new TranslateTransform();
            _selection.RenderTransform = transform;
            DoubleAnimation anim1 =
                new DoubleAnimation(x0 * _colW, x1 * _colW, new Duration(TimeSpan.FromSeconds(0.15)));
            DoubleAnimation anim2 =
                new DoubleAnimation(y0 * _rowH, y1 * _rowH, new Duration(TimeSpan.FromSeconds(0.15)));
            anim1.EasingFunction = new SineEase();
            anim2.EasingFunction = new SineEase();
            transform.BeginAnimation(TranslateTransform.XProperty, anim1);
            transform.BeginAnimation(TranslateTransform.YProperty, anim2);
        }

        /// <summary>
        /// Stretch animation for multiple selection
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        private void UpdateSelection(int width, int height, int newWidth, int newHeight)
        {
            if (_selection.Width != newWidth || _selection.Height != newHeight)
            {
                SetLeft(_selection, _selX * _colW);
                SetTop(_selection, _selY * _rowH);
                DoubleAnimation anim1 = new DoubleAnimation(width, newWidth, new Duration(TimeSpan.FromSeconds(0.05)));
                DoubleAnimation anim2 =
                    new DoubleAnimation(height, newHeight, new Duration(TimeSpan.FromSeconds(0.05)));
                anim1.EasingFunction = new SineEase();
                anim2.EasingFunction = new SineEase();
                _selection.BeginAnimation(WidthProperty, anim1);
                _selection.BeginAnimation(HeightProperty, anim2);
            }
        }

        /// <summary>
        /// Returns the cell at a certain point
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public Cell GetCell(int col, int row)
        {
            foreach (Cell cell in _cells)
                if (cell.Col == col && cell.Row == row)
                    return cell;
            return AddCell(col, row);
        }

        /// <summary>
        /// Returns the cell at a certain point
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public Cell Cells(int col, int row)
        {
            return GetCell(col, row);
        }

        /// <summary>
        /// Returns the selected cell
        /// </summary>
        /// <returns></returns>
        public Cell GetSelected()
        {
            Cell c = GetCell(_selX, _selY);
            if (c == null) return AddCell(_selX, _selY);
            return c;
        }
    }
}