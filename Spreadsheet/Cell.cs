using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Spreadsheet.Properties;

namespace Spreadsheet
{
    public class Cell
    {
        /// <summary>
        /// Puts a Textbox into the Cell
        /// </summary>
        public string Text
        {
            get
            {
                if (txt == null) return "";
                return txt.Text;
            }

            set
            {
                if (txt == null)
                {
                    txt = new TextBox();
                    txt.Width = Width;
                    txt.Height = Height;
                    Thickness txtPadding = txt.Padding;
                    txt.BorderBrush = null;
                    txt.BorderThickness = new Thickness(0);
                    txtPadding.Left = PaddingLeft;
                    txtPadding.Top = PaddingTop;
                    txt.BorderBrush = null;
                    txt.Padding = new Thickness(5, 4, 0, 0);
                    txt.Background = null;
                    txt.Foreground = TextColor;
                    txt.Focusable = false;
                    txt.Cursor = Cursors.Arrow;
                    Canvas.SetLeft(txt, Width * Col);
                    Canvas.SetTop(txt, Height * Row);
                    _canvas.Children.Add(txt);
                }

                txt.Text = value;

            }
        }

        /// <summary>
        /// Enables or disables the edit mode on a cell
        /// </summary>
        public bool Edit
        {
            get => _edit;
            set
            {
                if (!Editable) return;
                _edit = value;
                if (value)
                {
                    if (txt == null) Text = "";
                    txt.Cursor = Cursors.IBeam;
                    txt.Focusable = true;
                    txt.Focus();
                    txt.CaptureMouse();
                }
                else
                {
                    if (txt != null)
                    {
                        txt.Focusable = false;
                        txt.Cursor = Cursors.Arrow;
                    }
                }
            }
        }

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }

        public int Col = 0;
        public int Row = 0;

        public int PaddingLeft = 3;
        public int PaddingTop = 6;

        private TextBox txt;

        public bool Editable = true;

        public int Width = 0;
        public int Height = 0;

        private bool _selected = false;
        private bool _edit = false;

        private Canvas _canvas;

        public Brush TextColor = Brushes.Black;

        /// <summary>
        /// Constructor for the Cell
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="c"></param>
        public Cell(int col, int row, Canvas c)
        {
            Col = col;
            Row = row;

            _canvas = c;
        }
    }
}
