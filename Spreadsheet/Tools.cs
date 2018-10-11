using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace Spreadsheet
{
    class Tools
    {
        public static Brush Blend(Brush color, Brush backColor, double amount)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(Blend(BrushToColor(color), BrushToColor(backColor), amount).ToString()));
        }

        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromRgb(r, g, b);
        }

        public static Color BrushToColor(Brush brush)
        {
            return ((SolidColorBrush)brush).Color;
        }
    }
}
